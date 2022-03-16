using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HRSuite.Core;
using HRSuite.Core.Models;
using HRSuite.Core.Processors;
using HRSuite.Core.Reporting;

using MahApps.Metro.Controls.Dialogs;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Requisition.Module.ViewModels;
public class AddRequisitionViewModel : BindableBase, INavigationAware
{
    private readonly IDataProcessor _dataProcessor;
    private readonly IWorksheet _worksheet;
    private readonly IRegionManager _regionManager;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IErrorHandler _errorHandler;

    #region Add Requisition View Properties

    public static string Title => "Add Requisition";

    private RequisitionModel _requisition = new();
    public RequisitionModel Requisition
    {
        get => _requisition;
        set => SetProperty(ref _requisition, value);
    }

    private ObservableCollection<JobCodeModel> _jobCodeOptions = new();
    public ObservableCollection<JobCodeModel> JobCodeOptions
    {
        get => _jobCodeOptions;
        set => SetProperty(ref _jobCodeOptions, value);
    }

    private DateTime _deadlineDate = DateTime.Today.AddDays(8);
    public DateTime DeadlineDate
    {
        get => _deadlineDate;
        set => SetProperty(ref _deadlineDate, value);
    }

    private string _hiringSupervisor;
    public string HiringSupervisor
    {
        get => _hiringSupervisor;
        set => SetProperty(ref _hiringSupervisor, value);
    }

    #endregion

    #region Delegate Commands

    public DelegateCommand SaveCommand => new(Save);
    public DelegateCommand CancelCommand => new(Cancel);

    #endregion

    #region Constructor

    public AddRequisitionViewModel(IRegionManager regionManager, IDialogCoordinator dialogCoordinator, IErrorHandler errorHandler, IDataProcessor dataProcessor, IWorksheet worksheet)
    {
        _regionManager = regionManager;
        _dialogCoordinator = dialogCoordinator;
        _errorHandler = errorHandler;
        _dataProcessor = dataProcessor;
        _worksheet = worksheet;
    }

    #endregion

    #region Methods

    #region Navigation

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) => Requisition = new();
    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        Requisition = new()
        {
            Description = string.Empty,
            Status = "Open",
            ReasonOpen = string.Empty,
            ReasonClosed = string.Empty,
            NumPos = 1,
            CreateDate = DateTime.Now,
            LastChangeDate = DateTime.Now,
            OpenDate = DateTime.Today,
            CloseDate = new DateTime(1900, 1, 1),
            Comment = string.Empty
        };

        JobCodeOptions = new((await _dataProcessor.GetAllJobCodesAsync()).Where(_ => _.Depreciated == false));
        DeadlineDate = DateTime.Now.AddDays(8);
        HiringSupervisor = string.Empty;
    }

    #endregion

    #region Private

    private async void Save()
    {
        string title, message;
        if (Requisition.JobCode is null || Requisition.NumPos < 1)
        {
            title = "Error";
            message = "Job code can't be missing and/or number of positions must be greated than 0";
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }
        else
        {
            if (await _dialogCoordinator.ShowMessageAsync(this, "Confirm", "Are you sure you want to save this requisition?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
            {
                return;
            }

            title = "Please wait...";
            message = "Saving Requisition Information...";
            ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, true);
            controller.SetMessage(message);
            await Task.Run(() => Thread.Sleep(1));
            controller.SetIndeterminate();

            var transferToEdit = false;

            try
            {
                using CancellationTokenSource _tokensource = new();
                controller.Canceled += (object? sender, EventArgs e) =>
                {
                    message = "Cancelling";
                    controller.SetMessage(message);
                    _tokensource.Cancel();
                };
                Requisition.Id = await _dataProcessor.GetNewRequisitionIdAsync();
                Requisition.LastChangeDate = DateTime.Now;
                JobCodeModel jobCode = JobCodeOptions.FirstOrDefault(_ => _.Code == Requisition.JobCode);

                var dept = (await _dataProcessor.GetCodesByType("DEPT")).FirstOrDefault(_ => _.Code == jobCode.Department).Description;
                jobCode.Department = dept;

                await _dataProcessor.CreateRequisitionAsync(Requisition);

                message = "Creating Postings";
                controller.SetMessage(message);
                await Task.Run(() => Thread.Sleep(1));

                await Task.Run(() => _worksheet.CreateVacancyPosting(Requisition, jobCode, DeadlineDate, HiringSupervisor, true, _tokensource.Token));

                title = "Completed";
                message = $"Requisition was successfully saved as {Requisition.Code}.";
                transferToEdit = true;

            }
            catch (OperationCanceledException cancelEx)
            {
                title = "Cancelled";
                message = cancelEx.Message;
            }
            catch (Exception ex)
            {
                title = "Error";
                message = $"There is a problem saving the requisition.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
            }
            finally
            {
                await controller.CloseAsync();
                await _dialogCoordinator.ShowMessageAsync(this, title, message);
            }

            if (transferToEdit)
            {
                NavigationParameters parameters = new() { { "Requisition", Requisition } };
                _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.EditRequisitionView, parameters);
            }
        }
    }

    private void Cancel() => _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.AboutView);


    #endregion

    #endregion
}