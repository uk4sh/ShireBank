using static ShireBank.Protos.Inspector;

namespace ShireBank.Inspector
{
    internal class InspectorService : InspectorInterface
    {
        private readonly InspectorClient _client;

        public InspectorService(InspectorClient client)
        {
            _client = client;
        }

        public void FinishInspection() =>
            _client.FinishInspection(new Protos.FinishInspectionRequest());

        public string GetFullSummary() =>
            _client.GetFullSummary(new Protos.GetFullSummaryRequest()).Summary;

        public void StartInspection() =>
            _client.StartInspection(new Protos.StartInspectioRequest());
    }
}
