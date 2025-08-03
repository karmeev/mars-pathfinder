namespace Nasa.Dashboard.View.Internal.Core;

internal interface IViewFactory
{
    IView Create<T>() where T : IView;
}