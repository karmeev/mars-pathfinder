using System.Threading.Channels;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;
using Nasa.Pathfinder.Infrastructure.Internal.Grpc;

namespace Nasa.Pathfinder.Infrastructure;

public static class Registry
{
    public static void Register(IServiceCollection services)
    {
        services.AddMemoryCache();
    }
    
    public static void Register(ContainerBuilder builder)
    {
        builder.RegisterType<OperatorStream>().As<IOperatorStream>().SingleInstance();
        
        var capacity = new BoundedChannelOptions(100);
        builder.Register(_ => Channel.CreateBounded<SendMessageRequest>(capacity))
            .As<Channel<SendMessageRequest>>()
            .SingleInstance();
        builder.Register(c => c.Resolve<Channel<SendMessageRequest>>().Writer)
            .As<ChannelWriter<SendMessageRequest>>()
            .SingleInstance();
    }
}