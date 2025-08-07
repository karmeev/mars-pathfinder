using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Nasa.Pathfinder.Services;
using Registry = Nasa.Pathfinder.Registry;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000, o => o.Protocols = HttpProtocols.Http2); // insecure HTTP/2
});

Registry.RegisterServices(builder.Services);
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
    Registry.RegisterContainer(container));

var app = builder.Build();
app.MapGrpcService<PathfinderGrpcService>();
app.Run();