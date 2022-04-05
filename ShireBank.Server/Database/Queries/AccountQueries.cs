using Microsoft.EntityFrameworkCore;
using ShireBank.Server.Database.Queries.Interfaces;
using ShireBank.Server.Models;
using ShireBank.Server.Models.Enums;

namespace ShireBank.Server.Database.Queries
{
    public class AccountQueries : IAccountQueries
    {
        private readonly DataContext _context;

        public AccountQueries(DataContext context)
        {
            _context = context;
        }

        public async Task<Account> OpenAccount(string firstName, string lastName, float debitLimit)
        {
            if (debitLimit < 0) throw new ArgumentOutOfRangeException(nameof(debitLimit));
            if (firstName == null) throw new ArgumentNullException(nameof(firstName));
            if (lastName == null) throw new ArgumentNullException(nameof(lastName));

            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.FirstName == firstName && a.LastName == lastName);
            if (existingAccount != null)
            {
                if (existingAccount.IsClosed == false) throw new Exception($"The account with name '{firstName} {lastName}' exists and is already open.");

                existingAccount.IsClosed = false;
                await _context.SaveChangesAsync();
                return existingAccount;
            }

            var newAccount = new Account { FirstName = firstName, LastName = lastName, DebtLimit = debitLimit };
            await _context.Accounts.AddAsync(newAccount);
            await _context.SaveChangesAsync();
            return newAccount;
        }

        public async Task DepositFunds(uint accountId, float amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) throw new Exception($"Failed to find the account with id '{accountId}'.");

            var newBalance = account.Balance + amount;
            account.Balance = newBalance;

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Value = amount,
                Type = TransactionType.Deposit,
                Balance = newBalance
            };
            await _context.Transactions.AddAsync(transaction);

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        }

        public async Task<float> WithdrawFunds(uint accountId, float amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) throw new Exception($"Failed to find the account with id '{accountId}'.");

            var availableFunds = account.Balance + account.DebtLimit;
            if (availableFunds <= 0) return 0;

            var amountToWithdraw = amount > availableFunds ? availableFunds : amount;

            var newBalance = account.Balance - amountToWithdraw;
            account.Balance = newBalance;

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Value = amountToWithdraw,
                Type = TransactionType.Withdraw,
                Balance = newBalance
            };
            await _context.Transactions.AddAsync(transaction);

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
            return amountToWithdraw;
        }

        public async Task<Account> GetAccountWithTransactions(uint accountId) =>
            await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == accountId);

        public async Task<bool> CloseAccount(uint accountId)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Id == accountId);
            if (account == null) throw new Exception($"Failed to find the account with id '{accountId}'.");
            if (account.Balance != 0) return false;

            account.IsClosed = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsWithTransactions() =>
            await _context.Accounts
                .Include(a => a.Transactions)
                .ToArrayAsync();
    }
}
