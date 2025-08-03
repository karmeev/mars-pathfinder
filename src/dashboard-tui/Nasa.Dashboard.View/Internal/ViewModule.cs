using Autofac;
using Nasa.Dashboard.State;
using Nasa.Dashboard.View.Internal.Core;
using Nasa.Dashboard.View.Internal.Views;

namespace Nasa.Dashboard.View.Internal;

internal class ViewModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AppState>().AsSelf().SingleInstance();
        builder.RegisterType<ViewFactory>().As<IViewFactory>().SingleInstance();
        builder.RegisterType<MainMenuView>();
        builder.RegisterType<AcquireBotView>();
        builder.RegisterType<DrivingView>();
    }
}