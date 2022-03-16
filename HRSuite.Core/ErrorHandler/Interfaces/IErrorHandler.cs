
namespace HRSuite.Core;

public interface IErrorHandler
{
    /// <summary>
    /// Report an error with the exception info attached
    /// </summary>
    /// <param name="ex">The exception</param>
    /// <returns>The error message for further display</returns>
    Task<string> ReportErrorAsync(Exception ex);
}