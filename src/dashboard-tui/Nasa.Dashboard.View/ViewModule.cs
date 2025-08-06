using Autofac;
using Nasa.Dashboard.State;
using Nasa.Dashboard.View.Internal.Core;
using Nasa.Dashboard.View.Internal.Views;
using Nasa.Dashboard.View.Internal.Views.Engine;

namespace Nasa.Dashboard.View;

public class ViewModule : Module
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