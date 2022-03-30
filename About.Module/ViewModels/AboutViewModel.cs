using System;
using System.IO;

using Prism.Mvvm;
using Prism.Regions;

namespace About.Module.ViewModels;
public class AboutViewModel : BindableBase, INavigationAware
{
    private readonly string _dfsPath = Path.Combine(@"\\DomainServer", "dfs", "Custom Installs", "HR Suite", "PatchNotes.txt");
    private readonly string _hardPath = Path.Combine(@"\\LocalServer", "Custom Installs", "HR Suite", "PatchNotes.txt");

    #region About View Properties

    private string _message = $"Welcome, {Environment.UserName[..1].ToUpper()}{Environment.UserName[1..].ToLower()}!\r\nWhat would you like to do today?";
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    private string _patchNotes;
    public string PatchNotes
    {
        get => _patchNotes;
        set => SetProperty(ref _patchNotes, value);
    }

    #endregion

    #region Delegate Commands

    #endregion

    #region Constructor

    public AboutViewModel() => PatchNotes = File.Exists(_dfsPath) ? File.ReadAllText(_dfsPath) : File.Exists(_hardPath) ? File.ReadAllText(_hardPath) : "";

    #endregion

    #region Methods

    #region Navigation

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
    public void OnNavigatedTo(NavigationContext navigationContext) { }

    #endregion

    #region Private

    #endregion

    #endregion
}