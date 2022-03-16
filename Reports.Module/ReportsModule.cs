using Prism.Ioc;
using Prism.Modularity;

namespace Reports.Module;
public class ReportsModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<AgeListReportView>();
        containerRegistry.RegisterForNavigation<BirthdayListReportView>();
        containerRegistry.RegisterForNavigation<CDLReportView>();
        containerRegistry.RegisterForNavigation<EmployeeChecklistReportView>();
        containerRegistry.RegisterForNavigation<EmployeeListReportView>();
        containerRegistry.RegisterForNavigation<PromotionsReportView>();
        containerRegistry.RegisterForNavigation<RequisitionProfileReportView>();
        containerRegistry.RegisterForNavigation<SafetyMeetingAttendanceListReportView>();
        containerRegistry.RegisterForNavigation<ServiceListReportView>();
        containerRegistry.RegisterForNavigation<NewHireHeadcountDetailReportView>();
        containerRegistry.RegisterForNavigation<NewHireHeadcountSummaryReportView>();
        containerRegistry.RegisterForNavigation<EEORequisitionProfileReportView>();
        containerRegistry.RegisterForNavigation<TerminationDetailReportView>();
        containerRegistry.RegisterForNavigation<TerminationSummaryReportView>();
    }
}