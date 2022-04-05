using Grpc.Core;
using ShireBank.Protos;
using ShireBank.Server.Database.Queries.Interfaces;

namespace ShireBank.Server.Services
{
    public class InspectorService : Inspector.InspectorBase
    {
        private static int _isInspectionInProgressValue = 0;
        private readonly ILogger _logger;
        private readonly IAccountQueries _accountQueries;

        public InspectorService(ILogger<CustomerService> logger, IAccountQueries accountQueries)
        {
            _logger = logger;
            _accountQueries = accountQueries;
        }

        public override Task<FinishInspectionResponse> FinishInspection(FinishInspectionRequest request, ServerCallContext context)
        {
            IsInspectionInProgress = false;
            _logger.LogInformation("The inspection has ended.");
            return Task.FromResult(new FinishInspectionResponse());
        }

        public override async Task<GetFullSummaryResponse> GetFullSummary(GetFullSummaryRequest request, ServerCallContext context)
        {

            try
            {
                var accounts = await _accountQueries.GetAllAccountsWithTransactions();
                return new GetFullSummaryResponse { Summary = string.Join("\n", accounts) };
            }
            catch
            {
                _logger.LogWarning("Failed to get the summary...");
                throw;
            }
        }

        public override Task<StartInspectioResponse> StartInspection(StartInspectioRequest request, ServerCallContext context)
        {
            IsInspectionInProgress = true;
            _logger.LogInformation("The inspection has started.");
            return Task.FromResult(new StartInspectioResponse());
        }

        public static bool IsInspectionInProgress
        {
            get { return (Interlocked.CompareExchange(ref _isInspectionInProgressValue, 1, 1) == 1); }
            private set
            {
                if (value) Interlocked.CompareExchange(ref _isInspectionInProgressValue, 1, 0);
                else Interlocked.CompareExchange(ref _isInspectionInProgressValue, 0, 1);
            }
        }
    }
}
