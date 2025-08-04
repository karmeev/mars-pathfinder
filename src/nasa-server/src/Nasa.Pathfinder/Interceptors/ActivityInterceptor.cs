using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Nasa.Pathfinder.Interceptors;

public class ActivityInterceptor(ILogger<ActivityInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, 
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        logger.LogInformation("started; gRPC call: {Method}", context.Method);
        try
        {
            var headers = context.RequestHeaders;
            var traceId = headers.GetValue("traceId") ?? Guid.NewGuid().ToString();
            
            var response = await continuation(request, context);
            
            var responseHeaders = new Metadata
            {
                { "traceId", traceId }
            };
            await context.WriteResponseHeadersAsync(responseHeaders);
            
            logger.LogInformation("completed; gRPC call completed: {Method}", context.Method);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "failed; gRPC call failed: {Method}", context.Method);
            throw;
        }
    }
}