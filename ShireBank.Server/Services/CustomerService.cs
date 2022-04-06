using Grpc.Core;
using ShireBank.Protos;
using ShireBank.Server.Database.Handlers;
using ShireBank.Server.Database.Queries.Interfaces;

namespace ShireBank.Server.Services
{
    public class CustomerService : Customer.CustomerBase
    {
        private readonly ILogger _logger;
        private readonly IAccountQueries _accountQueries;
        private readonly IResilientQueryHandler _resilientQueryHandler;

        public CustomerService(ILogger<CustomerService> logger, IAccountQueries accountQueries, IResilientQueryHandler resilientQueryHandler)
        {
            _logger = logger;
            _accountQueries = accountQueries;
            _resilientQueryHandler = resilientQueryHandler;
        }

        public override async Task<CloseAccountResponse> CloseAccount(CloseAccountRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _accountQueries.CloseAccount(request.AccountId);
                return new CloseAccountResponse { IsSuccessful = result };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to close the account...");
                throw;
            }
        }

        public override async Task<DepositResponse> Deposit(DepositRequest request, ServerCallContext context)
        {
            try
            {
                await _resilientQueryHandler.HandleAsync(async () => await _accountQueries.DepositFunds(request.AccountId, request.Amount));
                return new DepositResponse();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deposit funds...");
                throw;
            }
        }

        public override async Task<GetHistoryResponse> GetHistory(GetHistoryRequest request, ServerCallContext context)
        {
            try
            {
                var account = await _accountQueries.GetAccountWithTransactions(request.AccountId);
                return new GetHistoryResponse { History = account.ToString() };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get history...");
                throw;
            }
        }

        public override async Task<OpenAccountResponse> OpenAccount(OpenAccountRequest request, ServerCallContext context)
        {
            try
            {
                var newAccount = await _accountQueries.OpenAccount(request.FirstName, request.LastName, request.DebtLimit);
                return new OpenAccountResponse { AccountId = newAccount.Id };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create the account...");
                return new OpenAccountResponse();
            }
        }

        public override async Task<WithdrawResponse> Withdraw(WithdrawRequest request, ServerCallContext context)
        {
            try
            {
                var withdrawnAmount = await _resilientQueryHandler.HandleAsync(async () => await _accountQueries.WithdrawFunds(request.AccountId, request.Amount));
                return new WithdrawResponse { WithdrawnAmount = withdrawnAmount };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to withdraw funds...");
                throw;
            }
        }
    }
}
