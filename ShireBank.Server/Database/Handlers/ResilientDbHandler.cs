using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ShireBank.Server.Database.Handlers
{
    public class ResilientDbHandler : IResilientDbHandler
    {
        private readonly TimeSpan[] _retryIntervals = new[]
        {
            TimeSpan.FromMilliseconds(50),
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(250),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromSeconds(1),
        };

        public async Task<T> HandleAsync<T>(Func<Task<T>> func)
        {
            var retryCount = 0;

            while (true)
            {
                try
                {
                    return await func.Invoke();
                }
                catch (Exception ex) when (ex is DbUpdateConcurrencyException || ex is DbException)
                {
                    if (++retryCount > _retryIntervals.Length) throw;

                    await Task.Delay(_retryIntervals[retryCount - 1]);
                }
            }
        }

        public async Task HandleAsync(Func<Task> func)
        {
            var retryCount = 0;

            while (true)
            {
                try
                {
                    await func.Invoke();
                    return;
                }
                catch (Exception ex) when (ex is DbUpdateConcurrencyException || ex is DbException)
                {
                    if (++retryCount > _retryIntervals.Length) throw;

                    await Task.Delay(_retryIntervals[retryCount - 1]);
                }
            }
        }
    }
}
