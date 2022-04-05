using Grpc.Net.Client;
using ShireBank.Inspector;
using ShireBank.Protos;
using ShireBank.Shared;

using var channel = GrpcChannel.ForAddress(Constants.FullBankAddress);

var inspectorClient = new Inspector.InspectorClient(channel);

var inspector = new InspectorService(inspectorClient);

inspector.StartInspection();

var summary = inspector.GetFullSummary();
Console.WriteLine(summary); // use NLOG

Console.ReadKey();
inspector.FinishInspection();
