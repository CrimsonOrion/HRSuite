using Prism.Ioc;
using Prism.Modularity;

using Requisition.Module.Views;

namespace Requisition.Module;
public class RequisitionModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<AddRequisitionView>();
        containerRegistry.RegisterForNavigation<EditRequisitionView>();
        containerRegistry.RegisterForNavigation<ApplicantView>();
    }
}