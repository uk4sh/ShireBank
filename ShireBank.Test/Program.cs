using Grpc.Net.Client;
using ShireBank.Protos;
using ShireBank.Shared;

using (var channel = GrpcChannel.ForAddress(Constants.FullBankAddress))
{
    ManualResetEvent[] endOfWorkEvents =
        { new ManualResetEvent(false), new ManualResetEvent(false), new ManualResetEvent(false) };

    var historyPrintLock = new object();

    // Customer 1
    new Thread(() =>
    {
        var customer = new Customer.CustomerClient(channel);

        Thread.Sleep(TimeSpan.FromSeconds(10));
        var accountId = customer.OpenAccount(new OpenAccountRequest { FirstName = "Henrietta", LastName = "Baggins", DebtLimit = 100.0f }).AccountId;
        if (accountId == null)
        {
            throw new Exception("Failed to open account");
        }

        customer.Deposit(new DepositRequest { AccountId = accountId.Value, Amount = 500.0f });

        Thread.Sleep(TimeSpan.FromSeconds(10));

        customer.Deposit(new DepositRequest { AccountId = accountId.Value, Amount = 500.0f });
        customer.Deposit(new DepositRequest { AccountId = accountId.Value, Amount = 1000.0f });

        if (2000.0f != customer.Withdraw(new WithdrawRequest { AccountId = accountId.Value, Amount = 2000.0f }).WithdrawnAmount)
        {
            throw new Exception("Can't withdraw a valid amount");
        }

        lock (historyPrintLock)
        {
            Console.WriteLine("=== Customer 1 ===");
            Console.Write(customer.GetHistory(new GetHistoryRequest { AccountId = accountId.Value }).History);
        }

        if (!customer.CloseAccount(new CloseAccountRequest { AccountId = accountId.Value }).IsSuccessful)
        {
            throw new Exception("Failed to close account");
        }

        endOfWorkEvents[0].Set();
    }).Start();

    // Customer 2
    new Thread(() =>
    {
        var customer = new Customer.CustomerClient(channel);

        var accountId = customer.OpenAccount(new OpenAccountRequest { FirstName = "Barbara", LastName = "Tuk", DebtLimit = 50.0f }).AccountId;
        if (accountId == null)
        {
            throw new Exception("Failed to open account");
        }

        if (customer.OpenAccount(new OpenAccountRequest { FirstName = "Barbara", LastName = "Tuk", DebtLimit = 500.0f }).AccountId != null)
        {
            throw new Exception("Opened account for the same name twice!");
        }

        if (50.0f != customer.Withdraw(new WithdrawRequest { AccountId = accountId.Value, Amount = 2000.0f }).WithdrawnAmount)
        {
            throw new Exception("Can only borrow up to debit limit only");
        }

        Thread.Sleep(TimeSpan.FromSeconds(10));

        if (customer.CloseAccount(new CloseAccountRequest { AccountId = accountId.Value }).IsSuccessful)
        {
            throw new Exception("Can't close the account with outstanding debt");
        }

        customer.Deposit(new DepositRequest { AccountId = accountId.Value, Amount = 100.0f });
        if (customer.CloseAccount(new CloseAccountRequest { AccountId = accountId.Value }).IsSuccessful)
        {
            throw new Exception("Can't close the account before clearing all funds");
        }

        if (50.0f != customer.Withdraw(new WithdrawRequest { AccountId = accountId.Value, Amount = 50.0f }).WithdrawnAmount)
        {
            throw new Exception("Can't withdraw a valid amount");
        }

        lock (historyPrintLock)
        {
            Console.WriteLine("=== Customer 2 ===");
            Console.Write(customer.GetHistory(new GetHistoryRequest { AccountId = accountId.Value }).History);
        }

        if (!customer.CloseAccount(new CloseAccountRequest { AccountId = accountId.Value }).IsSuccessful)
        {
            throw new Exception("Failed to close account");
            ;
        }

        endOfWorkEvents[1].Set();
    }).Start();


    // Customer 3
    new Thread(() =>
    {
        var customer = new Customer.CustomerClient(channel);

        var accountId = customer.OpenAccount(new OpenAccountRequest { FirstName = "Gandalf", LastName = "Grey", DebtLimit = 10000.0f }).AccountId;
        if (accountId == null)
        {
            throw new Exception("Failed to open account");
        }

        var toProcess = 200;
        var resetEvent = new ManualResetEvent(false);

        for (var i = 0; i < 100; i++)
        {
            ThreadPool.QueueUserWorkItem(stateInfo =>
            {
                if (customer.Withdraw(new WithdrawRequest { AccountId = accountId.Value, Amount = 10.0f }).WithdrawnAmount != 10.0f)
                {
                    throw new Exception("Can't withdraw a valid amount!");
                }

                if (Interlocked.Decrement(ref toProcess) == 0)
                {
                    resetEvent.Set();
                }
            });
        }

        for (var i = 0; i < 100; i++)
        {
            ThreadPool.QueueUserWorkItem(stateInfo =>
            {
                customer.Deposit(new DepositRequest { AccountId = accountId.Value, Amount = 10.0f });
                if (Interlocked.Decrement(ref toProcess) == 0)
                {
                    resetEvent.Set();
                }
            });
        }

        Thread.Sleep(TimeSpan.FromSeconds(10));

        resetEvent.WaitOne();

        lock (historyPrintLock)
        {
            Console.WriteLine("=== Customer 3 ===");
            Console.Write(customer.GetHistory(new GetHistoryRequest { AccountId = accountId.Value }).History);
        }

        if (!customer.CloseAccount(new CloseAccountRequest { AccountId = accountId.Value }).IsSuccessful)
        {
            throw new Exception("Failed to close account");
            ;
        }

        endOfWorkEvents[2].Set();
    }).Start();

    WaitHandle.WaitAll(endOfWorkEvents);
}

Console.ReadKey();
