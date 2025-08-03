using Autofac;
using Nasa.Pathfinder.Data.Internal;

namespace Nasa.Pathfinder.Data;

public class Registry
{
    public static void Register(ContainerBuilder builder)
    {
        builder.RegisterModule<DataModule>();
    }
}