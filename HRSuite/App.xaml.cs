using System;
using System.IO;
using System.Windows;

using About.Module;

using Codes.Module;

using ControlzEx.Theming;

using Employee.Module;

using HRSuite.Core;
using HRSuite.Core.ConfigurationModels;
using HRSuite.Core.Processors;
using HRSuite.Core.Reporting;

using JobCode.Module;

using Library.NET.DataAccess;
using Library.NET.Events;
using Library.NET.Logging;
using Library.NET.Mailer;

using MahApps.Metro.Controls.Dialogs;

using Microsoft.Extensions.Configuration;

using Prism.Ioc;
using Prism.Modularity;

using Reports.Module;

using Requisition.Module;

namespace HRSuite;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override Window CreateShell() => Container.Resolve<MainWindow>();
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
        ThemeManager.Current.SyncTheme();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Set up the custom logger to go to Desktop
        ICustomLogger logger = new CustomLogger(
            new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "HR Suite Error.log")),
            false,
            LogLevel.Warning);

        // Set up the configuration using the appSettings.json file.
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json", false, true)
            .Build();

        GlobalConfig.FileLocationsConfiguration = new()
        {
            LetterheadBwDfs = configuration["File Locations:Letterhead-BW-DFS"] ?? "",
            LetterheadBwHardcode = configuration["File Locations:Letterhead-BW-Hardcode"] ?? "",
        };

        GlobalConfig.EmailSettingsConfiguration = new()
        {
            SmtpServer = configuration["Email Settings:Smtp Server"] ?? "",
            AdminEmail = configuration["Email Settings:Admin Email"] ?? "",
            AdminText = configuration["Email Settings:Admin Text"] ?? ""
        };

        containerRegistry
                .RegisterInstance(logger)
                .RegisterInstance<IDialogCoordinator>(new DialogCoordinator())

                .Register<IEmailer, FluentEmailerMailKit>()
                .Register<IErrorHandler, ErrorHandler>()
                .Register<ISqlDataAccess, MsSqlDataAccess>()
                .Register<IDataProcessor, DataProcessor>()
                .Register<IWorksheet, Worksheet>()
                .Register<ILetter, Letter>()

                .RegisterSingleton<ISendMessageEvent, SendMessageEvent>()
                ;
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) => moduleCatalog
        .AddModule<AboutModule>()
        .AddModule<CodesModule>()
        .AddModule<EmployeeModule>()
        .AddModule<JobCodeModule>()
        .AddModule<ReportsModule>()
        .AddModule<RequisitionModule>()
        ;
}