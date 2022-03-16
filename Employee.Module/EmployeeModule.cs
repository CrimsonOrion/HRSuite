using Employee.Module.Views;

using Prism.Ioc;
using Prism.Modularity;

namespace Employee.Module;
public class EmployeeModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<EditEmployeeView>();
        containerRegistry.RegisterForNavigation<TerminateEmployeeView>();
    }
}