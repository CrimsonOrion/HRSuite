using System;
using System.Collections.Generic;
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

namespace Reports.Module.ViewModels;
public class TerminationSummaryReportViewModel : BindableBase, INavigationAware
{
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IErrorHandler _errorHandler;
    private readonly IDataProcessor _dataProcessor;
    private readonly IWorksheet _worksheet;

    #region Termination Headcount Summary Report View Properties

    public static string Title => "Termination Headcount Summary Report";

    private DateTime _startDate = DateTime.Today;
    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    private DateTime _endDate = DateTime.Today;
    public DateTime EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    private bool _isOfficeVisible = true;
    public bool IsOfficeVisible
    {
        get => _isOfficeVisible;
        set => SetProperty(ref _isOfficeVisible, value);
    }

    #endregion

    #region Delegate Commands

    public DelegateCommand CreateReportCommand => new(CreateReport);

    #endregion

    #region Constructor

    public TerminationSummaryReportViewModel(IDialogCoordinator dialogCoordinator, IErrorHandler errorHandler, IDataProcessor dataProcessor, IWorksheet worksheet)
    {
        _dialogCoordinator = dialogCoordinator;
        _errorHandler = errorHandler;
        _dataProcessor = dataProcessor;
        _worksheet = worksheet;
    }

    #endregion

    #region Methods

    #region Navigation

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
    public void OnNavigatedTo(NavigationContext navigationContext) { }

    #endregion

    #region Private

    private async void CreateReport()
    {
        if (await _dialogCoordinator.ShowMessageAsync(this, "Confirm", $"Are you sure you want to create the {Title}?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        var title = "Please wait...";
        var message = $"Getting {Title} Information...";
        ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, true);
        controller.SetMessage(message);
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

            // Get Data
            IEnumerable<EEOHeadcountDetailReportModel> data = await _dataProcessor.GetTerminatedHeadcount(StartDate, EndDate);
            await Task.Run(() => Thread.Sleep(1));

            // Create Report
            message = "Creating report...";
            controller.SetMessage(message);
            await Task.Run(() => Thread.Sleep(1));
            await Task.Run(() => _worksheet.CreateEEOHeadcountSummaryReport(data.ToList(), StartDate, EndDate, false, _isOfficeVisible, _tokensource.Token));

            title = "Completed";
            message = $"Report complete. Located on your Desktop as {Title}.xlsx.";
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (NullReferenceException)
        {
            title = "Error";
            message = "No data found.";
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem creating the {Title}.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
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