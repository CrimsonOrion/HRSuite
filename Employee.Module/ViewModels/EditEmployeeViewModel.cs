using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HRSuite.Core;
using HRSuite.Core.Models;
using HRSuite.Core.Processors;

using MahApps.Metro.Controls.Dialogs;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Employee.Module.ViewModels;
public class EditEmployeeViewModel : BindableBase, INavigationAware
{
    private readonly IRegionManager _regionManager;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IErrorHandler _errorHandler;
    private readonly IDataProcessor _dataProcessor;

    #region View Properties

    private string _title = "Edit Employee";
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private ObservableCollection<EmployeeModel> _employeeList;
    public ObservableCollection<EmployeeModel> EmployeeList
    {
        get => _employeeList;
        set => SetProperty(ref _employeeList, value);
    }

    private EmployeeModel _selectedEmployee;
    public EmployeeModel SelectedEmployee
    {
        get => _selectedEmployee;
        set
        {
            SetProperty(ref _selectedEmployee, value);
            SelectedEmployeeJobHistory = new(Task.Run(() => GetJobHistory()).Result);
        }
    }

    private ObservableCollection<EmployeeJobHistoryModel> _selectedEmployeeJobHistory;
    public ObservableCollection<EmployeeJobHistoryModel> SelectedEmployeeJobHistory
    {
        get => _selectedEmployeeJobHistory;
        set => SetProperty(ref _selectedEmployeeJobHistory, value);
    }

    private ObservableCollection<CodeModel> _stateOptions;
    public ObservableCollection<CodeModel> StateOptions
    {
        get => _stateOptions;
        set => SetProperty(ref _stateOptions, value);
    }

    private ObservableCollection<CodeModel> _sexOptions;
    public ObservableCollection<CodeModel> SexOptions
    {
        get => _sexOptions;
        set => SetProperty(ref _sexOptions, value);
    }

    private ObservableCollection<CodeModel> _ethnicityOptions;
    public ObservableCollection<CodeModel> EthnicityOptions
    {
        get => _ethnicityOptions;
        set => SetProperty(ref _ethnicityOptions, value);
    }

    private ObservableCollection<CodeModel> _disabledOptions;
    public ObservableCollection<CodeModel> DisabledOptions
    {
        get => _disabledOptions;
        set => SetProperty(ref _disabledOptions, value);
    }

    private ObservableCollection<CodeModel> _protectedVetOptions;
    public ObservableCollection<CodeModel> ProtectedVetOptions
    {
        get => _protectedVetOptions;
        set => SetProperty(ref _protectedVetOptions, value);
    }

    private ObservableCollection<CodeModel> _disabledVetOptions;
    public ObservableCollection<CodeModel> DisabledVetOptions
    {
        get => _disabledVetOptions;
        set => SetProperty(ref _disabledVetOptions, value);
    }

    private ObservableCollection<string> _cdlOptions = new() { "CDL", "Non-CDL" };
    public ObservableCollection<string> CDLOptions
    {
        get => _cdlOptions;
        set => SetProperty(ref _cdlOptions, value);
    }

    private ObservableCollection<string> _cdlDriverTypeOptions = new() { "", "Excepted Interstate", "Non-Excepted Interstate", "Excepted Intrastate", "Non-Excepted Intrastate" };
    public ObservableCollection<string> CDLDriverTypeOptions
    {
        get => _cdlDriverTypeOptions;
        set => SetProperty(ref _cdlDriverTypeOptions, value);
    }

    #endregion

    #region Delegate Commands

    public DelegateCommand SaveCommand => new(Save);
    public DelegateCommand CancelCommand => new(Cancel);

    #endregion

    #region Constructor

    public EditEmployeeViewModel(IRegionManager regionManager, IDialogCoordinator dialogCoordinator, IErrorHandler errorHandler, IDataProcessor dataProcessor)
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
        Title = string.Empty;
        SelectedEmployee = new();
    }
    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        IEnumerable<CodeModel> codes = await _dataProcessor.GetAllCodesAsync();
        StateOptions = new(codes.Where(_ => _.Type == "STATE"));
        SexOptions = new(codes.Where(_ => _.Type == "GENDER"));
        EthnicityOptions = new(codes.Where(_ => _.Type == "ETHNIC"));
        DisabledOptions = ProtectedVetOptions = DisabledVetOptions = new(codes.Where(_ => _.Type == "STATUS"));
        EmployeeList = new((await _dataProcessor.GetAllActiveEmployeesAsync()).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName).ThenBy(_ => _.MiddleName).ThenBy(_ => _.Id));

        if (navigationContext.Parameters.Any())
        {
            var empId = navigationContext.Parameters.GetValue<int>("EmployeeId");
            SelectedEmployee = EmployeeList.FirstOrDefault(_ => _.Id == empId);
        }
    }

    #endregion

    #region Private

    private async void Save()
    {
        var title = "Confirm";
        var message = "Are you sure you want to update this employee?";

        if (await _dialogCoordinator.ShowMessageAsync(this, title, message, MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        title = "Please wait...";
        message = "Updating Employee Information...";
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

            await _dataProcessor.UpdateEmployeeByIdAsync(SelectedEmployee);

            title = "Completed";
            message = $"Employee was successfully updated.";
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem updating the employee.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }
    }

    private void Cancel() => _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.AboutView);

    private async Task<List<EmployeeJobHistoryModel>> GetJobHistory() => new(await _dataProcessor.GetJobHistoryByEmployeeIdAsync(SelectedEmployee.Id));

    #endregion

    #endregion
}