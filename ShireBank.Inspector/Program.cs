using Grpc.Net.Client;
using ShireBank.Inspector;
using ShireBank.Protos;
using ShireBank.Shared;

var logger = NLog.LogManager.GetCurrentClassLogger();

using var channel = GrpcChannel.ForAddress(Constants.FullBankAddress);

var inspectorClient = new Inspector.InspectorClient(channel);

var inspector = new InspectorService(inspectorClient);

inspector.StartInspection();

var summary = inspector.GetFullSummary();
logger.Info(summary); // use NLOG

Console.ReadKey();
inspector.FinishInspection();
