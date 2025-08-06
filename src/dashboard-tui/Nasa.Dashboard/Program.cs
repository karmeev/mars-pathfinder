using Autofac;
using Nasa.Dashboard.Clients;
using Nasa.Dashboard.Internal;
using Nasa.Dashboard.Reducers;
using Nasa.Dashboard.View;

var builder = new ContainerBuilder();
builder.RegisterModule<ClientsModule>()
    .RegisterModule<ViewModule>()
    .RegisterModule<ReducersModule>();

var container = builder.Build();
using var scope = container.BeginLifetimeScope();

var app = new NasaApp();
app.InitializeViews(scope);
app.Run();