using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

using HRSuite.Core;
using HRSuite.Core.Models;
using HRSuite.Core.Processors;

using MahApps.Metro.Controls.Dialogs;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Codes.Module.ViewModels;
public class EditCodeViewModel : BindableBase, INavigationAware
{
    private readonly IDataProcessor _dataProcessor;
    private readonly IRegionManager _regionManager;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IErrorHandler _errorHandler;

    #region Edit Code View Properties

    public static string Title => "Edit Code";

    private ObservableCollection<CodeModel> _codesList;
    public ObservableCollection<CodeModel> CodesList
    {
        get => _codesList;
        set => SetProperty(ref _codesList, value);
    }

    private CodeModel _selectedCode = new();
    public CodeModel SelectedCode
    {
        get => _selectedCode;
        set => SetProperty(ref _selectedCode, value);
    }

    private ObservableCollection<string> _typeOptions;
    public ObservableCollection<string> TypeOptions
    {
        get => _typeOptions;
        set => SetProperty(ref _typeOptions, value);
    }

    private string _description;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    #endregion

    #region Delegate Commands

    public DelegateCommand SaveCommand => new(Save);
    public DelegateCommand CancelCommand => new(Cancel);
    public DelegateCommand DeleteCommand => new(Delete);

    #endregion

    #region Constructor

    public EditCodeViewModel(IRegionManager regionManager, IDialogCoordinator dialogCoordinator, IErrorHandler errorHandler, IDataProcessor dataProcessor)
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
    public void OnNavigatedFrom(NavigationContext navigationContext) { SelectedCode = null; TypeOptions = null; }
    public async void OnNavigatedTo(NavigationContext navigationContext) { CodesList = new(await _dataProcessor.GetAllCodesAsync()); TypeOptions = new(await _dataProcessor.GetTypesAsync()); SelectedCode = new(); }

    #endregion

    #region Private

    private async void Save()
    {
        if (await _dialogCoordinator.ShowMessageAsync(this, "Confirm", "Are you sure you want to save this code?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        var title = "Please wait...";
        var message = "Saving Code Information...";
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

            await _dataProcessor.UpdateCodeByIdAsync(SelectedCode);

            title = "Completed";
            message = $"Code was successfully saved as {SelectedCode.FullDescription}.";
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem saving the code.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
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
        if (await _dialogCoordinator.ShowMessageAsync(this, "Confirm", "Are you sure you want to delete this code?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
        {
            return;
        }

        var title = "Please wait...";
        var message = "Deleting Code Information...";
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

            await _dataProcessor.DeleteCodeByIdAsync(SelectedCode);

            title = "Completed";
            message = $"Code {SelectedCode.FullDescription} was successfully deleted.";
        }
        catch (OperationCanceledException cancelEx)
        {
            title = "Cancelled";
            message = cancelEx.Message;
        }
        catch (Exception ex)
        {
            title = "Error";
            message = $"There is a problem saving the code.\r\n{await _errorHandler.ReportErrorAsync(ex)}";
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