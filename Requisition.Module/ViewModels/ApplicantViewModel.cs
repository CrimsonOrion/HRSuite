using System;
using System.Collections.Generic;
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
public class ApplicantViewModel : BindableBase, INavigationAware
{
    private readonly IRegionManager _regionManager;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IErrorHandler _errorHandler;
    private readonly IDataProcessor _dataProcessor;
    private readonly ILetter _letter;

    #region View Properties

    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private ApplicantModel _applicant;
    public ApplicantModel Applicant
    {
        get => _applicant;
        set => SetProperty(ref _applicant, value);
    }

    private RequisitionModel _requisition;
    public RequisitionModel Requisition
    {
        get => _requisition;
        set => SetProperty(ref _requisition, value);
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

    private ObservableCollection<CodeModel> _applicationStatusOptions;
    public ObservableCollection<CodeModel> ApplicationStatusOptions
    {
        get => _applicationStatusOptions;
        set => SetProperty(ref _applicationStatusOptions, value);
    }

    private ObservableCollection<CodeModel> _rejectionOptions;
    public ObservableCollection<CodeModel> RejectionOptions
    {
        get => _rejectionOptions;
        set => SetProperty(ref _rejectionOptions, value);
    }

    public int EmployeeId { get; set; }

    #endregion

    #region Delegate Commands

    public DelegateCommand SaveCommand => new(Save);
    public DelegateCommand CancelCommand => new(Cancel);

    #endregion

    #region Constructor

    public ApplicantViewModel(IRegionManager regionManager, IDialogCoordinator dialogCoordinator, IErrorHandler errorHandler, IDataProcessor dataProcessor, ILetter letter)
    {
        _regionManager = regionManager;
        _dialogCoordinator = dialogCoordinator;
        _errorHandler = errorHandler;
        _dataProcessor = dataProcessor;
        _letter = letter;
    }

    #endregion

    #region Methods

    #region Navigation

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
        Title = string.Empty;
        Requisition = null;
        Applicant = null;
    }
    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        IEnumerable<CodeModel> codes = await _dataProcessor.GetAllCodesAsync();
        StateOptions = new(codes.Where(_ => _.Type == "STATE"));
        SexOptions = new(codes.Where(_ => _.Type == "GENDER"));
        EthnicityOptions = new(codes.Where(_ => _.Type == "ETHNIC"));
        DisabledOptions = ProtectedVetOptions = DisabledVetOptions = new(codes.Where(_ => _.Type == "STATUS"));
        ApplicationStatusOptions = new(codes.Where(_ => _.Type == "APPSTAT"));
        RejectionOptions = new(codes.Where(_ => _.Type == "REJECT"));

        if (navigationContext.Parameters.Any())
        {
            Title = navigationContext.Parameters.GetValue<string>("Title");
            Requisition = navigationContext.Parameters.GetValue<RequisitionModel>("Requisition");
            Applicant = navigationContext.Parameters.GetValue<ApplicantModel>("Applicant");
            EmployeeId = navigationContext.Parameters.GetValue<int>("EmployeeId");
        }
    }

    #endregion

    #region Private

    private async void Save()
    {
        if (await _dialogCoordinator.ShowMessageAsync(this, "Confirm", "Are you sure you want to save this applicant?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        var title = "Please wait...";
        var message = "Saving Applicant Information...";
        ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, true);
        controller.SetMessage(message);
        await Task.Run(() => Thread.Sleep(1));
        controller.SetIndeterminate();

        var updateSuccess = false;
        try
        {
            using CancellationTokenSource _tokensource = new();
            controller.Canceled += (object? sender, EventArgs e) =>
            {
                message = "Cancelling";
                controller.SetMessage(message);
                _tokensource.Cancel();
            };

            // If the applicant is rejected, create a letter
            if (Applicant.GeneralStatus == "CMPREJ" && Applicant.LetterCode == string.Empty)
            {
                message = "Creating Rejection Letter";
                controller.SetMessage(message);
                await Task.Run(() => Thread.Sleep(1));

                var jobTitle = (await _dataProcessor.GetAllJobCodesAsync()).FirstOrDefault(_ => _.Code == Requisition.JobCode).JobTitle;
                await Task.Run(() => _letter.CreateRejectionLetter(Applicant, jobTitle, true, _tokensource.Token));

                Applicant.LetterCode = "REJECT";
                Applicant.LetterSendDate = DateTime.Today;
            }

            if (Title.Equals("Add Applicant"))
            {
                message = "Creating Applicant";
                controller.SetMessage(message);
                await Task.Run(() => Thread.Sleep(1));

                // Add the application to the requisition
                await _dataProcessor.CreateApplicantAsync(Applicant);

                if (Requisition.Internal)
                {
                    // Add the applicant to InternalApplicants
                    await _dataProcessor.CreateInternalApplicantAsync(new() { ApplicantId = Applicant.Id, EmployeeId = EmployeeId });
                }
            }
            else
            {
                message = "Updating Applicant";
                controller.SetMessage(message);
                await Task.Run(() => Thread.Sleep(1));

                // Update the application
                await _dataProcessor.UpdateApplicantAsync(Applicant);
            }

            message = "Succesfully Updated Applicant";
            updateSuccess = true;
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem saving the applicant.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
        }
        finally
        {
            await controller.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, title, message);
        }

        if (updateSuccess)
        {
            // Return to the EditRequisitionView, sending the updated Requisition.
            NavigationParameters parameters = new() { { "Requisition", Requisition } };
            _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.EditRequisitionView, parameters);
        }
    }

    private void Cancel()
    {
        // Return to the EditRequisitionView, sending the un-updated Requisition.
        NavigationParameters parameters = new() { { "Requisition", Requisition } };
        _regionManager.RequestNavigate(KnownRegionNames.MainRegion, KnownViewNames.EditRequisitionView, parameters);
    }

    #endregion

    #endregion
}