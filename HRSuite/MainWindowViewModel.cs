using HRSuite.Core;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace HRSuite;
public class MainWindowViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;

    #region Main Window Properties

    public static string Title => $"HR Suite v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";

    #endregion

    #region Delegate Commands

    #region Navigation

    #region Employees

    public DelegateCommand EditEmployeeScreenCommand => new(EditEmployeeScreen);
    public DelegateCommand TerminateEmployeeScreenCommand => new(TerminateEmployeeScreen);

    private void EditEmployeeScreen() => Navigate(KnownViewNames.EditEmployeeView);
    private void TerminateEmployeeScreen() => Navigate(KnownViewNames.TerminateEmployeeView);

    #endregion

    #region Requisitions

    public DelegateCommand CreateRequisitionScreenCommand => new(CreateRequisitionScreen);
    public DelegateCommand EditRequisitionScreenCommand => new(EditRequisitionScreen);

    private void CreateRequisitionScreen() => Navigate(KnownViewNames.AddRequisitionView);
    private void EditRequisitionScreen() => Navigate(KnownViewNames.EditRequisitionView);

    #endregion

    #region Code Data

    #region Job Codes

    public DelegateCommand AddJobCodeScreenCommand => new(AddJobCodeScreen);
    public DelegateCommand EditJobCodeScreenCommand => new(EditJobCodeScreen);

    private void AddJobCodeScreen() => Navigate(KnownViewNames.AddJobCodeView);
    private void EditJobCodeScreen() => Navigate(KnownViewNames.EditJobCodeView);

    #endregion

    #region Other Codes

    public DelegateCommand AddCodeScreenCommand => new(AddCodeScreen);
    public DelegateCommand EditCodeScreenCommand => new(EditCodeScreen);

    private void AddCodeScreen() => Navigate(KnownViewNames.AddCodeView);
    private void EditCodeScreen() => Navigate(KnownViewNames.EditCodeView);

    #endregion

    #endregion

    #region Reports

    #region Employee

    public DelegateCommand AgeListScreenCommand => new(AgeListScreen);
    public DelegateCommand BirthdayListScreenCommand => new(BirthdayListScreen);
    public DelegateCommand CDLReportScreenCommand => new(CDLReportScreen);
    public DelegateCommand EmployeeChecklistScreenCommand => new(EmployeeChecklistScreen);
    public DelegateCommand EmployeeListScreenCommand => new(EmployeeListScreen);
    public DelegateCommand PromotionsScreenCommand => new(PromotionsScreen);
    public DelegateCommand RequisitionProfileScreenCommand => new(RequisitionProfileScreen);
    public DelegateCommand SafetyMeetingAttendanceListScreenCommand => new(SafetyMeetingAttendanceListScreen);
    public DelegateCommand ServiceListScreenCommand => new(ServiceListScreen);

    private void AgeListScreen() => Navigate(KnownViewNames.AgeListReportView);
    private void BirthdayListScreen() => Navigate(KnownViewNames.BirthdayListReportView);
    private void CDLReportScreen() => Navigate(KnownViewNames.CDLReportView);
    private void EmployeeChecklistScreen() => Navigate(KnownViewNames.EmployeeChecklistReportView);
    private void EmployeeListScreen() => Navigate(KnownViewNames.EmployeeListReportView);
    private void PromotionsScreen() => Navigate(KnownViewNames.PromotionsReportView);
    private void RequisitionProfileScreen() => Navigate(KnownViewNames.RequisitionProfileReportView);
    private void SafetyMeetingAttendanceListScreen() => Navigate(KnownViewNames.SafetyMeetingAttendanceListReportView);
    private void ServiceListScreen() => Navigate(KnownViewNames.ServiceListReportView);

    #endregion

    #region EEO

    public DelegateCommand NewHireHeadcountDetailScreenCommand => new(NewHireHeadcountDetailScreen);
    public DelegateCommand NewHireHeadcountSummaryScreenCommand => new(NewHireHeadcountSummaryScreen);
    public DelegateCommand EEORequisitionProfileScreenCommand => new(EEORequisitionProfileScreen);
    public DelegateCommand TerminationDetailScreenCommand => new(TerminationDetailScreen);
    public DelegateCommand TerminationSummaryScreenCommand => new(TerminationSummaryScreen);

    private void NewHireHeadcountDetailScreen() => Navigate(KnownViewNames.NewHireHeadcountDetailReportView);
    private void NewHireHeadcountSummaryScreen() => Navigate(KnownViewNames.NewHireHeadcountSummaryReportView);
    private void EEORequisitionProfileScreen() => Navigate(KnownViewNames.EEORequisitionProfileReportView);
    private void TerminationDetailScreen() => Navigate(KnownViewNames.TerminationDetailReportView);
    private void TerminationSummaryScreen() => Navigate(KnownViewNames.TerminationSummaryReportView);

    #endregion

    #endregion

    #region About

    public DelegateCommand AboutScreenCommand => new(AboutScreen);

    private void AboutScreen() => Navigate(KnownViewNames.AboutView);

    #endregion

    #endregion

    #endregion

    #region Constructor

    public MainWindowViewModel(IRegionManager regionManager) => _regionManager = regionManager;

    #endregion

    #region Private Methods

    private void Navigate(string navigationPath, NavigationParameters navigationParameters = null) => _regionManager.RequestNavigate(KnownRegionNames.MainRegion, navigationPath, navigationParameters);

    #endregion
}