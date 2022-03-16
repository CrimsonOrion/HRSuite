using System;
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
public class TerminateEmployeeViewModel : BindableBase, INavigationAware
{
    private readonly IRegionManager _regionManager;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IErrorHandler _errorHandler;
    private readonly IDataProcessor _dataProcessor;

    #region Terminate Employee View Properties

    public static string Title => "Terminate Employee";

    private string _terminateButtonText;
    public string TerminateButtonText
    {
        get => _terminateButtonText;
        set => SetProperty(ref _terminateButtonText, value);
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
            SetProperty(ref _selectedEmployee, value ?? new());
            TerminateButtonText = SelectedEmployee.Active ? "Terminate" : "Un-Terminate";
            TerminationDate = SelectedEmployee.Active ? DateTime.Today : SelectedEmployee.TerminationDate;
        }
    }

    private ObservableCollection<CodeModel> _terminationOptions;
    public ObservableCollection<CodeModel> TerminationOptions
    {
        get => _terminationOptions;
        set => SetProperty(ref _terminationOptions, value);
    }

    private DateTime _terminationDate;
    public DateTime TerminationDate
    {
        get => _terminationDate;
        set => SetProperty(ref _terminationDate, value);
    }


    #endregion

    #region Delegate Commands

    public DelegateCommand SaveCommand => new(Save);
    public DelegateCommand CancelCommand => new(Cancel);

    #endregion

    #region Constructor

    public TerminateEmployeeViewModel(IRegionManager regionManager, IDialogCoordinator dialogCoordinator, IErrorHandler errorHandler, IDataProcessor dataProcessor)
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
    public void OnNavigatedFrom(NavigationContext navigationContext) => SelectedEmployee = new();
    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        EmployeeList = new((await _dataProcessor.GetAllEmployeesAsync()).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName));
        TerminationOptions = new((await _dataProcessor.GetAllCodesAsync()).Where(_ => _.Type == "TERM"));
        SelectedEmployee = new();
    }

    #endregion

    #region Private

    private async void Save()
    {
        var prefix = SelectedEmployee.Active ? "" : "un-";
        if (await _dialogCoordinator.ShowMessageAsync(this, "Confirm", $"Are you sure you want to {prefix}terminate {SelectedEmployee.FullName}?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        var title = "Please wait...";
        var message = SelectedEmployee.Active ? $"Terminating Employee..." : "Un-Terminating Employee...";
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

            await _dataProcessor.UpdateEmployeeActiveStatusAsync(SelectedEmployee.Id, !SelectedEmployee.Active, SelectedEmployee.TerminationCode, TerminationDate);

            title = "Completed";
            message = $"{SelectedEmployee.FullName} was successfully {prefix}terminated.";

            EmployeeModel tempSelectedEmployee = SelectedEmployee;
            EmployeeList = new((await _dataProcessor.GetAllEmployeesAsync()).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName));
            SelectedEmployee = EmployeeList.FirstOrDefault(_ => _.Id == tempSelectedEmployee.Id);
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem {prefix}terminating the employee.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }
    }

    private void Cancel() => _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.AboutView);

    #endregion

    #endregion
}