using JobCode.Module.Views;

using Prism.Ioc;
using Prism.Modularity;

namespace JobCode.Module;
public class JobCodeModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<AddJobCodeView>();
        containerRegistry.RegisterForNavigation<EditJobCodeView>();
    }
}