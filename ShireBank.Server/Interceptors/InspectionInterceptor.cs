using Grpc.Core;
using Grpc.Core.Interceptors;
using ShireBank.Server.Services;

namespace ShireBank.Server.Interceptors
{
    public class InspectionInterceptor : Interceptor
    {
        private readonly ILogger _logger;

        public InspectionInterceptor(ILogger<InspectionInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (InspectorService.IsInspectionInProgress)
            {
                _logger.LogWarning($"The call was blocked because the inspection is in progress. Type: {MethodType.Unary}. " + $"Method: {context.Method}.");
                throw new Exception("You cannot perform this operation when the inspection is in progress.");
            }

            return await continuation(request, context);
        }
    }
}
