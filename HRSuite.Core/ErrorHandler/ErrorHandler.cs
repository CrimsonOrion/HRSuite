using System.DirectoryServices.AccountManagement;

using HRSuite.Core.ConfigurationModels;

using Library.NET.Logging;
using Library.NET.Mailer;
using Library.NET.Mailer.Models;

namespace HRSuite.Core;
public class ErrorHandler : IErrorHandler
{
    private readonly string _name;
    private readonly string _emailAddress;
    private readonly string _adminEmail;
    private readonly string _smtpServer;
    private readonly IEmailer _emailer;
    private readonly ICustomLogger _logger;

    #region Constructor

    public ErrorHandler(IEmailer emailer, ICustomLogger logger)
    {
#pragma warning disable CA1416 // Validate platform compatibility
        UserPrincipal userInfo = UserPrincipal.Current;
        _name = userInfo.GivenName + " " + userInfo.Surname;
        _emailAddress = userInfo.EmailAddress;
#pragma warning restore CA1416 // Validate platform compatibility
        _adminEmail = GlobalConfig.EmailSettingsConfiguration.AdminEmail;
        _smtpServer = GlobalConfig.EmailSettingsConfiguration.SmtpServer;
        _emailer = emailer;
        _logger = logger;
    }

    #endregion

    #region Methods

    #region Public

    public async Task<string> ReportErrorAsync(Exception ex)
    {
        var errorMessage = $"Error as follows: {ex.Message}\r\nInner Exception:{ex.InnerException}";
        try
        {
            ResponseModel result = await _emailer.SetOptions(
                new AddressModel(_emailAddress, _name),
                new List<AddressModel>() { new AddressModel(_adminEmail) },
                "Error in HR Suite",
                errorMessage + $"\r\nStackTrace:\r\n{ex.StackTrace}",
                _smtpServer
                ).SendEmailAsync();
            _logger.LogError(ex, errorMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Problem sending mail: {e.Message}\r\n{e.InnerException}");
        }

        return errorMessage;
    }

    #endregion

    #endregion
}