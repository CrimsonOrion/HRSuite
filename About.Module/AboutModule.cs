using About.Module.Views;

using HRSuite.Core;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace About.Module;
public class AboutModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) => containerProvider
        .Resolve<IRegionManager>()
        .RegisterViewWithRegion(KnownRegionNames.MainRegion, typeof(AboutView));

    public void RegisterTypes(IContainerRegistry containerRegistry) => containerRegistry.RegisterForNavigation<AboutView>();
}