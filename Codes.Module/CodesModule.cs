using Codes.Module.Views;

using Prism.Ioc;
using Prism.Modularity;

namespace Codes.Module;
public class CodesModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<AddCodeView>();
        containerRegistry.RegisterForNavigation<EditCodeView>();
    }
}