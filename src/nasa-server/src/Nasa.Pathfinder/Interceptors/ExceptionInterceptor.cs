using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Options;
using Nasa.Pathfinder.Settings;

namespace Nasa.Pathfinder.Interceptors;

public class ExceptionInterceptor(
    ILogger<ExceptionInterceptor> logger,
    IOptions<EnableSettings> settings) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            var trailers = new Metadata
            {
                { "error_code", ex.StatusCode.ToString() },
                { "error_message", ex.Message }
            };

            throw new RpcException(ex.Status, trailers);
        }
        catch (Exception ex)
        {
            var status = new Status(StatusCode.Internal, "Internal server error");
            var trailers = new Metadata
            {
                { "error_code", nameof(StatusCode.Internal) },
                { "error_message", "Internal server error" }
            };

            if (settings.Value.EnableExceptionInResponse)
            {
                trailers.Add(new Metadata.Entry("internal_error", ex.Message));
                trailers.Add(new Metadata.Entry("internal_stack_trace", ex?.StackTrace ?? "Stack Trace is null"));
            }

            throw new RpcException(status, trailers);
        }
    }
}