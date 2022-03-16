using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using HRSuite.Core.Models;

using Prism.Commands;

using Prism.Mvvm;

namespace Requisition.Module.Dialogs;
public class InternalHireReasonDialogViewModel : BindableBase
{
    private readonly Action<InternalHireReasonDialogViewModel> _closeHandler;

    #region Internal Hire Reason Dialog View Properties

    public CodeModel Result { get; set; }

    private ObservableCollection<CodeModel> _jobChangeReasonOptions;
    public ObservableCollection<CodeModel> JobChangeReasonOptions
    {
        get => _jobChangeReasonOptions;
        set => SetProperty(ref _jobChangeReasonOptions, value);
    }

    private CodeModel _selectedJobChangeReason;
    public CodeModel SelectedJobChangeReason
    {
        get => _selectedJobChangeReason;
        set => SetProperty(ref _selectedJobChangeReason, value);
    }

    #endregion

    #region Delegate Commands

    public DelegateCommand<InternalHireReasonDialogViewModel> CloseJobChangeReasonCommand => new(_closeHandler, _ => true);
    public DelegateCommand AddCommand => new(Add);
    public DelegateCommand CancelCommand => new(Cancel);

    #endregion

    #region Constructor

    public InternalHireReasonDialogViewModel(List<CodeModel> jobChangeReasonOptions, Action<InternalHireReasonDialogViewModel> closeHandler)
    {
        JobChangeReasonOptions = new(jobChangeReasonOptions);
        _closeHandler = closeHandler;
    }

    #endregion

    #region Methods

    private void Add()
    {
        if (SelectedJobChangeReason == null)
        {
            return;
        }

        Result = SelectedJobChangeReason;
        CloseJobChangeReasonCommand.Execute(this);
    }

    private void Cancel()
    {
        Result = null;
        CloseJobChangeReasonCommand.Execute(this);
    }

    #endregion
}