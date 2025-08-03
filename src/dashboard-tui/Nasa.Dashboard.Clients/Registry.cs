using Autofac;
using Nasa.Dashboard.Clients.Internal;

namespace Nasa.Dashboard.Clients;

public static class Registry
{
    public static void Register(this ContainerBuilder builder)
    {
        builder.RegisterModule<ClientsModule>();
    }
}