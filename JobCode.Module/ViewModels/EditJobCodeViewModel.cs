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

namespace JobCode.Module.ViewModels;
public class EditJobCodeViewModel : BindableBase, INavigationAware
{
    private readonly IDataProcessor _dataProcessor;
    private readonly IRegionManager _regionManager;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IErrorHandler _errorHandler;

    #region Edit Job Code View Properties

    public static string Title => "Edit Job Code";

    private List<JobCodeModel> _jobCodesList;
    public List<JobCodeModel> JobCodesList
    {
        get => _jobCodesList;
        set => SetProperty(ref _jobCodesList, value);
    }

    private JobCodeModel _selectedJobCode;
    public JobCodeModel SelectedJobCode
    {
        get => _selectedJobCode;
        set
        {
            SetProperty(ref _selectedJobCode, value ?? new());
            DepreciateButtonText = SelectedJobCode.Depreciated ? "Reinstate" : "Depreciate";
        }
    }

    private ObservableCollection<CodeModel> _eeoOptions;
    public ObservableCollection<CodeModel> EEOOptions
    {
        get => _eeoOptions;
        set => SetProperty(ref _eeoOptions, value);
    }

    private ObservableCollection<CodeModel> _departmentOptions;
    public ObservableCollection<CodeModel> DepartmentOptions
    {
        get => _departmentOptions;
        set => SetProperty(ref _departmentOptions, value);
    }

    private ObservableCollection<string> _insideOutsideOptions = new() { "Inside", "Outside" };
    public ObservableCollection<string> InsideOutsideOptions
    {
        get => _insideOutsideOptions;
        set => SetProperty(ref _insideOutsideOptions, value);
    }

    private ObservableCollection<JobCodeModel> _supervisorOptions;
    public ObservableCollection<JobCodeModel> SupervisorOptions
    {
        get => _supervisorOptions;
        set => SetProperty(ref _supervisorOptions, value);
    }

    private ObservableCollection<CodeModel> _requisitionTypeOptions;
    public ObservableCollection<CodeModel> RequisitionTypeOptions
    {
        get => _requisitionTypeOptions;
        set => SetProperty(ref _requisitionTypeOptions, value);
    }

    private string _description;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private string _depreciateButtonText = "Depreciate";
    public string DepreciateButtonText
    {
        get => _depreciateButtonText;
        set => SetProperty(ref _depreciateButtonText, value);
    }

    #endregion

    #region Delegate Commands

    public DelegateCommand SaveCommand => new(Save);
    public DelegateCommand CancelCommand => new(Cancel);
    public DelegateCommand DepreciateCommand => new(Depreciate);
    public DelegateCommand DeleteCommand => new(Delete);

    #endregion

    #region Constructor

    public EditJobCodeViewModel(IRegionManager regionManager, IDialogCoordinator dialogCoordinator, IErrorHandler errorHandler, IDataProcessor dataProcessor)
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
    public void OnNavigatedFrom(NavigationContext navigationContext) => SelectedJobCode = null;
    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        SelectedJobCode = new();
        JobCodesList = new(await _dataProcessor.GetAllJobCodesAsync());
        IEnumerable<CodeModel> codes = await _dataProcessor.GetAllCodesAsync();
        EEOOptions = new(codes.Where(_ => _.Type == "EEO"));
        DepartmentOptions = new(codes.Where(_ => _.Type == "DEPT"));
        SupervisorOptions = new(await _dataProcessor.GetAllJobCodesAsync());
        RequisitionTypeOptions = new(codes.Where(_ => _.Type == "REQTYPE"));
    }

    #endregion

    #region Private

    private async void Save()
    {
        if (await _dialogCoordinator.ShowMessageAsync(this, "Confirm", "Are you sure you want to save this job code?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        var title = "Please wait...";
        var message = "Saving Job Code Information...";
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

            await _dataProcessor.UpdateJobCodeByIdAsync(SelectedJobCode);

            title = "Completed";
            message = $"Job code was successfully updated as {SelectedJobCode.FullDescription}.";
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem updating the job code.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }
    }

    private void Cancel() => _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.AboutView);

    private async void Depreciate()
    {
        var depreciated = SelectedJobCode.Depreciated;
        var message = depreciated ? $"Reinstate Job Code {SelectedJobCode.FullDescription}?" : $"Depreciate Job Code {SelectedJobCode.FullDescription}?";

        if (await _dialogCoordinator.ShowMessageAsync(this, "Confirm", message, MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        var title = "Please wait...";
        message = depreciated ? $"Reinstating Job Code {SelectedJobCode.FullDescription}" : $"Depreciating Job Code {SelectedJobCode.FullDescription}";
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

            SelectedJobCode.JobTitle = depreciated ? SelectedJobCode.JobTitle.Replace(" (DEPR)", "") : SelectedJobCode.JobTitle + " (DEPR)";

            await _dataProcessor.UpdateJobCodeByIdAsync(SelectedJobCode);

            var action = depreciated ? "reinstated" : "depreciated";
            title = "Completed";
            message = $"Job code was successfully {action} as {SelectedJobCode.FullDescription}.";
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem updating the job code.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }
    }

    private async void Delete()
    {
        if (await _dialogCoordinator.ShowMessageAsync(this, "Confirm", "Are you sure you want to delete this job code?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        var title = "Please wait...";
        var message = "Saving Job Code Information...";
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

            IEnumerable<JobHistoryModel> jobCodeInUseBy = await _dataProcessor.GetJobHistoryByJobCodeAsync(SelectedJobCode.Code);

            if (jobCodeInUseBy.Any())
            {
                JobHistoryModel jobHistoryForExample = jobCodeInUseBy.FirstOrDefault();
                title = "Error";
                message = $"JobCode {SelectedJobCode.Code} currently in use in the job history for Employee ID: {jobHistoryForExample.EmployeeId}. You can not remove it.";
            }
            else
            {
                await _dataProcessor.DeleteJobCodeByIdAsync(SelectedJobCode);

                title = "Completed";
                message = $"Job code {SelectedJobCode.FullDescription} was successfully deleted.";
            }
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem updating the job code.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }
    }

    #endregion

    #endregion
}