using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

using HRSuite.Core;
using HRSuite.Core.Models;
using HRSuite.Core.Processors;

using MahApps.Metro.Controls.Dialogs;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using Requisition.Module.Dialogs;

namespace Requisition.Module.ViewModels;
public class EditRequisitionViewModel : BindableBase, INavigationAware
{
    private readonly IDataProcessor _dataProcessor;
    private readonly IRegionManager _regionManager;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IErrorHandler _errorHandler;

    #region View Properties

    public static string Title => "Edit Requisition";

    private List<RequisitionModel> _requisitionList;
    public List<RequisitionModel> RequisitionList
    {
        get => _requisitionList;
        set => SetProperty(ref _requisitionList, value);
    }

    private RequisitionModel _selectedRequisition = new();
    public RequisitionModel SelectedRequisition
    {
        get => _selectedRequisition;
        set
        {
            SetProperty(ref _selectedRequisition, value);
            if (SelectedRequisition.Id > 0)
            {
                LastChangedDate = $"Last Changed: {SelectedRequisition.LastChangeDate:yyyy-MM-dd}";
                ApplicantList = new(Task.Run(() => GetApplicantsAsync()).Result);
            }
        }
    }

    private string _lastChangedDate;
    public string LastChangedDate
    {
        get => _lastChangedDate;
        set => SetProperty(ref _lastChangedDate, value);
    }

    private ObservableCollection<JobCodeModel> _jobCodeOptions = new();
    public ObservableCollection<JobCodeModel> JobCodeOptions
    {
        get => _jobCodeOptions;
        set => SetProperty(ref _jobCodeOptions, value);
    }

    private ObservableCollection<string> _statusOptions = new() { "Open", "Closed" };
    public ObservableCollection<string> StatusOptions
    {
        get => _statusOptions;
        set => SetProperty(ref _statusOptions, value);
    }

    private ObservableCollection<ApplicantRequisitionListModel> _applicantList = new();
    public ObservableCollection<ApplicantRequisitionListModel> ApplicantList
    {
        get => _applicantList;
        set => SetProperty(ref _applicantList, value);
    }

    private ApplicantRequisitionListModel _selectedApplicant;
    public ApplicantRequisitionListModel SelectedApplicant
    {
        get => _selectedApplicant;
        set => SetProperty(ref _selectedApplicant, value);
    }

    private bool _rehired;
    public bool Rehired
    {
        get => _rehired;
        set => SetProperty(ref _rehired, value);
    }

    #endregion

    #region Delegate Commands

    public DelegateCommand SaveCommand => new(Save);
    public DelegateCommand CancelCommand => new(Cancel);
    public DelegateCommand DeleteCommand => new(Delete);
    public DelegateCommand AddApplicantCommand => new(AddApplicant);
    public DelegateCommand<DataGrid> DeleteApplicantCommand => new(DeleteApplicant);
    public DelegateCommand<DataGrid> EditApplicantCommand => new(EditApplicant);
    public DelegateCommand<DataGrid> HireApplicantCommand => new(HireApplicant);
    public DelegateCommand<DataGrid> UnHireApplicantCommand => new(UnHireApplicant);

    #endregion

    #region Constructor

    public EditRequisitionViewModel(IRegionManager regionManager, IDialogCoordinator dialogCoordinator, IErrorHandler errorHandler, IDataProcessor dataProcessor)
    {
        _regionManager = regionManager;
        _dialogCoordinator = dialogCoordinator;
        _errorHandler = errorHandler;
        _dataProcessor = dataProcessor;
    }

    #endregion

    #region Methods

    #region Navigation

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
        SelectedRequisition = new();
        LastChangedDate = string.Empty;
        ApplicantList.Clear();
    }

    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        JobCodeOptions = new((await _dataProcessor.GetAllJobCodesAsync()).Where(_ => _.Depreciated == false));
        RequisitionList = new(await _dataProcessor.GetAllRequisitionsAsync());

        if (navigationContext.Parameters.Any())
        {
            RequisitionModel req = navigationContext.Parameters.GetValue<RequisitionModel>("Requisition");
            SelectedRequisition = RequisitionList.FirstOrDefault(_ => _.Id == req.Id);
        }
    }

    #endregion

    #region Private

    private async void Save()
    {
        if (await _dialogCoordinator.ShowMessageAsync(this, "Confirm", "Are you sure you want to update this requisition?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        var title = "Please wait...";
        var message = "Updating Requisition Information...";
        ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, true);
        controller.SetMessage(message);
        await Task.Run(() => Thread.Sleep(1));
        controller.SetIndeterminate();

        try
        {
            using CancellationTokenSource _tokensource = new();
            controller.Canceled += (object? sender, EventArgs e) =>
            {
                message = "Cancelling";
                controller.SetMessage(message);
                _tokensource.Cancel();
            };

            await Task.Run(() => Thread.Sleep(1000));

            // Check if user wants to cancel the process
            if (_tokensource.Token.IsCancellationRequested)
            {
                _tokensource.Token.ThrowIfCancellationRequested();
            }

            SelectedRequisition.LastChangeDate = DateTime.Now;

            SelectedRequisition.CloseDate =
                SelectedRequisition.Status == "Closed" && SelectedRequisition.CloseDate == new DateTime(1900, 1, 1) ?
                DateTime.Today :
                SelectedRequisition.Status == "Open" && SelectedRequisition.CloseDate > new DateTime(1900, 1, 1) ?
                new DateTime(1900, 1, 1) :
                SelectedRequisition.CloseDate;

            await _dataProcessor.UpdateRequisitionAsync(SelectedRequisition);

            title = "Completed";
            message = $"Requisition was successfully updated.";
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem updating the requisition.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }
    }

    private void Cancel() => _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.AboutView);

    private async void Delete()
    {
        if (await _dialogCoordinator.ShowMessageAsync(this, "Confirm", "Are you sure you want to delete this requisition?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        if (ApplicantList.Any())
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Cannot Delete Requisition", "You can't delete a requisition that has applicants. Please remove the applicants, then delete the requisition.");
            return;
        }

        var title = "Please wait...";
        var message = "Deleting Requisition Information...";
        ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, true);
        controller.SetMessage(message);
        await Task.Run(() => Thread.Sleep(1));
        controller.SetIndeterminate();

        try
        {
            using CancellationTokenSource _tokensource = new();
            controller.Canceled += (object? sender, EventArgs e) =>
            {
                message = "Cancelling";
                controller.SetMessage(message);
                _tokensource.Cancel();
            };

            await Task.Run(() => Thread.Sleep(2000));

            // Check if user wants to cancel the process
            if (_tokensource.Token.IsCancellationRequested)
            {
                _tokensource.Token.ThrowIfCancellationRequested();
            }

            await _dataProcessor.DeleteRequisitionByIdAsync(SelectedRequisition.Id);

            title = "Completed";
            message = $"Requisition was successfully deleted.";
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem deleting the requisition.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }

        if (title.Equals("Completed"))
        {
            Cancel();
        }
    }

    private async void AddApplicant()
    {
        if (SelectedRequisition.Id == 0)
        {
            return;
        }

        ApplicantModel applicant = new();
        var title = "Add Applicant";
        var empId = 0;

        if (SelectedRequisition.Internal)
        {
            CustomDialog customDialog = new() { Title = title };

            IOrderedEnumerable<EmployeeModel> activeEmployees = (await _dataProcessor.GetAllActiveEmployeesAsync()).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName).ThenBy(_ => _.MiddleName).ThenBy(_ => _.Id);

            InternalApplicantDialogViewModel dataContext = new(new(activeEmployees), async _ =>
            {
                if (_.Result is not null)
                {
                    EmployeeModel internalApp = _.Result;
                    empId = internalApp.Id;
                    applicant = new()
                    {
                        Id = await _dataProcessor.GetNewApplicantIdAsync(),
                        RequisitionId = SelectedRequisition.Id,
                        FirstName = internalApp.FirstName,
                        MiddleName = internalApp.MiddleName,
                        LastName = internalApp.LastName,
                        Nickname = internalApp.Nickname,
                        Address1 = internalApp.Address1,
                        Address2 = internalApp.Address2,
                        City = internalApp.City,
                        State = internalApp.State,
                        Zip = internalApp.Zip,
                        HomePhone = internalApp.HomePhone,
                        CellPhone = internalApp.CellPhone,
                        Sex = internalApp.Sex,
                        Ethnicity = internalApp.Ethnicity,
                        Disabled = internalApp.Disabled,
                        ProtectedVet = internalApp.ProtectedVet,
                        DisabledVet = internalApp.DisabledVet,
                        ApplicationDate = DateTime.Today,
                        GeneralStatus = "NEW",
                        StatusAsOf = DateTime.Today,
                        Interviewed = false,
                        LetterCode = string.Empty,
                        LetterSendDate = new DateTime(1900, 1, 1),
                        AcceptDate = new DateTime(1900, 1, 1),
                        RejectionCode = string.Empty,
                        Comment = string.Empty
                    };
                }
                NavigationParameters parameters = new()
                {
                    { "Title", title },
                    { "Requisition", SelectedRequisition },
                    { "Applicant", applicant },
                    { "EmployeeId", empId }
                };

                // Have to close the dialog before you move on.
                await _dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                // Once closed, NOW you can move on to the next view.
                _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.ApplicantView, parameters);
            });

            customDialog.Content = new InternalApplicantDialogView { DataContext = dataContext };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }
        else
        {
            applicant = new()
            {
                Id = await _dataProcessor.GetNewApplicantIdAsync(),
                RequisitionId = SelectedRequisition.Id,
                FirstName = string.Empty,
                MiddleName = string.Empty,
                LastName = string.Empty,
                Nickname = string.Empty,
                Address1 = string.Empty,
                Address2 = string.Empty,
                City = string.Empty,
                State = string.Empty,
                Zip = string.Empty,
                HomePhone = string.Empty,
                CellPhone = string.Empty,
                Sex = string.Empty,
                Ethnicity = string.Empty,
                Disabled = string.Empty,
                ProtectedVet = string.Empty,
                DisabledVet = string.Empty,
                ApplicationDate = DateTime.Today,
                GeneralStatus = "NEW",
                StatusAsOf = DateTime.Today,
                Interviewed = false,
                LetterCode = string.Empty,
                LetterSendDate = new DateTime(1900, 1, 1),
                AcceptDate = new DateTime(1900, 1, 1),
                RejectionCode = string.Empty,
                Comment = string.Empty
            };

            NavigationParameters parameters = new()
            {
                { "Title", title },
                { "Requisition", SelectedRequisition },
                { "Applicant", applicant }
            };

            _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.ApplicantView, parameters);
        }
    }

    private async void DeleteApplicant(DataGrid dataGrid)
    {
        if (dataGrid.SelectedItem is not ApplicantRequisitionListModel applicant || await _dialogCoordinator.ShowMessageAsync(this, "Confirm", $"Are you sure you want to delete {applicant.FullName}?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        var title = "Please wait...";
        var message = "Deleting Applicant Information...";
        ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, true);
        controller.SetMessage(message);
        await Task.Run(() => Thread.Sleep(1));
        controller.SetIndeterminate();

        try
        {
            using CancellationTokenSource _tokensource = new();
            controller.Canceled += (object? sender, EventArgs e) =>
            {
                message = "Cancelling";
                controller.SetMessage(message);
                _tokensource.Cancel();
            };

            await Task.Run(() => Thread.Sleep(2000));

            // Check if user wants to cancel the process
            if (_tokensource.Token.IsCancellationRequested)
            {
                _tokensource.Token.ThrowIfCancellationRequested();
            }

            if (SelectedRequisition.Internal)
            {
                await _dataProcessor.DeleteInternalApplicantByApplicantIdAsync(applicant.ApplicantId);
            }

            await _dataProcessor.DeleteApplicantByIdAsync(applicant.ApplicantId);

            title = "Completed";
            message = $"Applicant was successfully deleted.";
            ApplicantList = new(await GetApplicantsAsync());
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem deleting the applicant.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }
    }

    private async void EditApplicant(DataGrid dataGrid)
    {
        if (dataGrid.SelectedItem is not ApplicantRequisitionListModel applicant)
        {
            return;
        }

        ApplicantModel selectedApplicant = await _dataProcessor.GetApplicantByIdAsync(applicant.ApplicantId);

        NavigationParameters parameters = new()
        {
            { "Title", "Edit Applicant" },
            { "Requisition", SelectedRequisition },
            { "Applicant", selectedApplicant }
        };
        _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.ApplicantView, parameters);
    }

    private async void HireApplicant(DataGrid dataGrid)
    {
        if (dataGrid.SelectedItem is not ApplicantRequisitionListModel applicant || await _dialogCoordinator.ShowMessageAsync(this, "Confirm", $"Are you sure you want to hire {applicant.FullName}?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        if (applicant.GeneralStatus == "OFFACC")
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Invalid Hire", "You can not hire an applicant a second time.");
            return;
        }

        if (SelectedRequisition.Internal)
        {
            CustomDialog customDialog = new() { Title = "Select Job Change Reason" };

            IEnumerable<CodeModel> jobChangeCodesList = (await _dataProcessor.GetAllCodesAsync()).Where(_ => _.Type == "JOBCHANGE");

            InternalHireReasonDialogViewModel dataContext = new(new(jobChangeCodesList), async _ =>
            {
                // Have to close the dialog before you move on.
                await _dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                if (_.Result is not null)
                {
                    await HireApplicantProcess(applicant.ApplicantId, _.Result.Code, true);
                }
            });

            customDialog.Content = new InternalHireReasonDialogView { DataContext = dataContext };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }
        else
        {
            await HireApplicantProcess(applicant.ApplicantId, "NEW HIRE", false);
        }
    }

    private async void UnHireApplicant(DataGrid dataGrid)
    {
        if (dataGrid.SelectedItem is not ApplicantRequisitionListModel applicant || await _dialogCoordinator.ShowMessageAsync(this, "Confirm", $"Are you sure you want to unhire {applicant.FullName}?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        if (applicant.GeneralStatus != "OFFACC")
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Invalid Unhire", "You can not unhire an applicant that is not hired.");
            return;
        }

        var title = "Please wait...";
        var message = "Reverting Applicant's Hire Information...";
        ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, true);
        controller.SetMessage(message);
        await Task.Run(() => Thread.Sleep(1));
        controller.SetIndeterminate();

        try
        {
            using CancellationTokenSource _tokensource = new();
            controller.Canceled += (object? sender, EventArgs e) =>
            {
                message = "Cancelling";
                controller.SetMessage(message);
                _tokensource.Cancel();
            };

            await Task.Run(() => Thread.Sleep(2000));

            // Check if user wants to cancel the process
            if (_tokensource.Token.IsCancellationRequested)
            {
                _tokensource.Token.ThrowIfCancellationRequested();
            }

            // Update Applicant
            ApplicantModel unhiredApplicant = await _dataProcessor.GetApplicantByIdAsync(applicant.ApplicantId);
            unhiredApplicant.AcceptDate = new DateTime(1900, 1, 1);
            unhiredApplicant.GeneralStatus = "NEW";
            unhiredApplicant.StatusAsOf = DateTime.Today;
            await _dataProcessor.UpdateApplicantAsync(unhiredApplicant);

            InternalApplicantModel internalApplicant = await _dataProcessor.GetInternalApplicantByApplicantIdAsync(applicant.ApplicantId);

            // Delete Job History
            JobHistoryModel jobHistory = new()
            {
                EmployeeId = internalApplicant.EmployeeId,
                JobCode = SelectedRequisition.JobCode
            };

            await _dataProcessor.DeleteJobHistoryAsync(jobHistory);

            // Delete Internal Applicant and Employee if not an existing employee
            if (!SelectedRequisition.Internal)
            {
                await _dataProcessor.DeleteInternalApplicantByApplicantIdAsync(applicant.ApplicantId);

                await _dataProcessor.DeleteEmployeeByIdAsync(jobHistory.EmployeeId);
            }

            title = "Completed";
            message = $"Applicant's hire status was successfully reverted.";
            ApplicantList = new(await GetApplicantsAsync());
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem reverting the applicant's hire status.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }
    }

    private async Task<List<ApplicantRequisitionListModel>> GetApplicantsAsync()
    {
        IEnumerable<ApplicantRequisitionListModel> list = await _dataProcessor.GetApplicantByRequisitionIdAsync(SelectedRequisition.Id);
        return list.ToList();
    }

    private async Task HireApplicantProcess(int applicantId, string reason, bool internalApplicant)
    {
        var title = "Please wait...";
        var message = "Updating Applicant Information...";
        ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, true);
        controller.SetMessage(message);
        await Task.Run(() => Thread.Sleep(1));
        controller.SetIndeterminate();

        var employeeId = 0;
        var hiredApplicantName = "";
        try
        {
            using CancellationTokenSource _tokensource = new();
            controller.Canceled += (object? sender, EventArgs e) =>
            {
                message = "Cancelling";
                controller.SetMessage(message);
                _tokensource.Cancel();
            };

            await Task.Run(() => Thread.Sleep(2000));

            // Check if user wants to cancel the process
            if (_tokensource.Token.IsCancellationRequested)
            {
                _tokensource.Token.ThrowIfCancellationRequested();
            }

            ApplicantModel hiredApplicant = await _dataProcessor.GetApplicantByIdAsync(applicantId);
            hiredApplicant.AcceptDate = DateTime.Today;
            hiredApplicant.GeneralStatus = "OFFACC";
            hiredApplicant.StatusAsOf = DateTime.Today;
            hiredApplicantName = hiredApplicant.FullName;
            await _dataProcessor.UpdateApplicantAsync(hiredApplicant);

            if (internalApplicant)
            {
                InternalApplicantModel internalApplicantInfo = await _dataProcessor.GetInternalApplicantByApplicantIdAsync(applicantId);
                employeeId = internalApplicantInfo.EmployeeId;
            }
            else
            {
                employeeId = await _dataProcessor.CreateEmployeeAsync(hiredApplicant);

                InternalApplicantModel internalApplicantInfo = new()
                {
                    EmployeeId = employeeId,
                    ApplicantId = applicantId
                };

                // add the new employee to the link table, InternalApplicants.
                await _dataProcessor.CreateInternalApplicantAsync(internalApplicantInfo);
            }

            JobHistoryModel jobHistory = new()
            {
                EmployeeId = employeeId,
                JobCode = SelectedRequisition.JobCode,
                ChangeDate = DateTime.Today,
                JobDate = DateTime.Today,
                Reason = reason
            };

            await _dataProcessor.CreateEmployeeJobHistoryAsync(jobHistory);

            title = "Completed";
            message = $"Applicant was successfully hired.";
            ApplicantList = new(await GetApplicantsAsync());
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem hiring the applicant.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);

            if (title.Equals("Completed") && await _dialogCoordinator.ShowMessageAsync(this, "Confirm", $"Would you like to edit employee information for {hiredApplicantName}?", MessageDialogStyle.AffirmativeAndNegative, new() { AffirmativeButtonText = "Yes", NegativeButtonText = "No" }) == MessageDialogResult.Affirmative)
            {
                NavigationParameters parameters = new()
                {
                    { "EmployeeId", employeeId }
                };
                _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.EditEmployeeView, parameters);
            }
        }
    }

    #endregion

    #endregion
}