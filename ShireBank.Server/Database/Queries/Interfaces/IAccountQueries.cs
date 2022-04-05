using ShireBank.Server.Models;

namespace ShireBank.Server.Database.Queries.Interfaces
{
    public interface IAccountQueries
    {
        Task<Account> OpenAccount(string firstName, string lastName, float debitLimit);
        Task DepositFunds(uint accountId, float amount);
        Task<float> WithdrawFunds(uint accountId, float amount);
        Task<Account> GetAccountWithTransactions(uint accountId);
        Task<IEnumerable<Account>> GetAllAccountsWithTransactions();
        Task<bool> CloseAccount(uint accountId);
    }
}