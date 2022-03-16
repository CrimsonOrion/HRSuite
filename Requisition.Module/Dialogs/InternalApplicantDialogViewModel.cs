using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using HRSuite.Core.Models;

using Prism.Commands;

using Prism.Mvvm;

namespace Requisition.Module.Dialogs;
public class InternalApplicantDialogViewModel : BindableBase
{
    private readonly Action<InternalApplicantDialogViewModel> _closeHandler;

    #region Internal Applicant Dialog View Properties

    public EmployeeModel Result { get; set; }

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
        set => SetProperty(ref _selectedEmployee, value);
    }

    #endregion

    #region Delegate Commands

    public DelegateCommand<InternalApplicantDialogViewModel> CloseAddApplicantCommand => new(_closeHandler, _ => true);
    public DelegateCommand AddCommand => new(Add);
    public DelegateCommand CancelCommand => new(Cancel);

    #endregion

    #region Constructor

    public InternalApplicantDialogViewModel(List<EmployeeModel> employeeList, Action<InternalApplicantDialogViewModel> closeHandler)
    {
        EmployeeList = new(employeeList);
        _closeHandler = closeHandler;
    }

    #endregion

    #region Methods

    private void Add()
    {
        if (SelectedEmployee == null)
        {
            return;
        }

        Result = SelectedEmployee;
        CloseAddApplicantCommand.Execute(this);
    }

    private void Cancel()
    {
        Result = null;
        CloseAddApplicantCommand.Execute(this);
    }

    #endregion
}