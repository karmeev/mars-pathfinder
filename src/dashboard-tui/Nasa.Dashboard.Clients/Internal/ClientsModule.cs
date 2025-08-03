using Autofac;
using Grpc.Net.Client;
using Nasa.Dashboard.Clients.Contracts;
using Nasa.Dashboard.Clients.Internal.Pathfinder;
using Pathfinder.Proto;

namespace Nasa.Dashboard.Clients.Internal;

internal class ClientsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(GrpcChannel.ForAddress("http://localhost:5000"))
            .As<GrpcChannel>().SingleInstance();

        builder.Register(ctx =>
        {
            var chan = ctx.Resolve<GrpcChannel>();
            return new PathfinderService.PathfinderServiceClient(chan);
        }).AsSelf().SingleInstance();
        
        builder.RegisterType<PathfinderClient>().As<IPathfinderClient>().SingleInstance();
    }
}