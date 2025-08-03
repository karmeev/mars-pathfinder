using Autofac;
using Nasa.Dashboard;
using Nasa.Dashboard.Internal;

var builder = new ContainerBuilder();
builder.RegisterDependencies();

var container = builder.Build();
using var scope = container.BeginLifetimeScope();

var app = new NasaApp();
app.InitializeViews(scope);
app.Run();