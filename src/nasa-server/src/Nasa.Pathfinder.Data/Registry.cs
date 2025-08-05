using Autofac;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Data.Internal;
using Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;

namespace Nasa.Pathfinder.Data;

public class Registry
{
    public static void Register(ContainerBuilder builder)
    {
        builder.Register(c =>
        {
            var memoryContext = c.Resolve<IMemoryDataContext>();
            return new BotRepository(memoryContext);
        }).As<IBotRepository>();
    }
}