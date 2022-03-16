using HRSuite.Core.Models;

namespace HRSuite.Core.Reporting;
public interface ILetter
{
    /// <summary>
    /// Creates applicant rejection letter
    /// </summary>
    /// <param name="model">Applicant information</param>
    /// <param name="positionTitle">Position the requisition was for</param>
    /// <param name="openWord">Open document upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateRejectionLetter(ApplicantModel model, string positionTitle, bool openWord, CancellationToken token);
}