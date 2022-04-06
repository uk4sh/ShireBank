
namespace ShireBank.Server.Database.Handlers
{
    public interface IResilientQueryHandler
    {
        Task HandleAsync(Func<Task> func);
        Task<T> HandleAsync<T>(Func<Task<T>> func);
    }
}