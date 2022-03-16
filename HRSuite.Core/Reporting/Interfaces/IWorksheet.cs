using HRSuite.Core.Models;

namespace HRSuite.Core.Reporting;
public interface IWorksheet
{
    #region Employee Reports

    /// <summary>
    /// Creates Age List Report
    /// </summary>
    /// <param name="models">Employee List</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateAgeListReport(List<EmployeeJobReportModel> models, bool openExcel, CancellationToken token);

    /// <summary>
    /// Creates Birthday List Report
    /// </summary>
    /// <param name="models">Employee List</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateBirthdayListReport(List<EmployeeModel> models, bool openExcel, CancellationToken token);

    /// <summary>
    /// Creates CDL Report
    /// </summary>
    /// <param name="models">Employee List</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateCDLReport(List<EmployeeModel> models, bool openExcel, CancellationToken token);

    /// <summary>
    /// Creates Employee Checklist
    /// </summary>
    /// <param name="models">Employee List</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateEmployeeChecklistReport(List<EmployeeModel> models, bool openExcel, CancellationToken token);

    /// <summary>
    /// Creates Employee List Report
    /// </summary>
    /// <param name="models">Employee List</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateEmployeeListReport(List<EmployeeJobReportModel> models, bool openExcel, CancellationToken token);

    /// <summary>
    /// Creates Birthday List Report
    /// </summary>
    /// <param name="models">Employee List</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreatePromotionsReport(List<PromotionReportModel> models, DateTime startDate, DateTime endDate, bool openExcel, CancellationToken token);

    /// <summary>
    /// Creates Requisition Profile Report
    /// </summary>
    /// <param name="requisition">Requisition information</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateRequisitionProfileReport(RequisitionProfileReportModel requisition, bool openExcel, CancellationToken token);

    /// <summary>
    /// Creates Safety Meeting Attendance List Report
    /// </summary>
    /// <param name="models">Employee List</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateSafetyMeetingAttenListReport(List<EmployeeJobReportModel> models, bool openExcel, CancellationToken token);

    /// <summary>
    /// Creates Years of Service List Report
    /// </summary>
    /// <param name="models">Employee List</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateServiceListReport(List<EmployeeJobReportModel> models, bool openExcel, CancellationToken token);

    #endregion

    #region EEO Reports

    /// <summary>
    /// Creates EEO headcount report for reporting EEO statistics
    /// </summary>
    /// <param name="model">Applicant Lists' EEO information</param>
    /// <param name="startDate">Report start date</param>
    /// <param name="endDate">Report end date</param>
    /// <param name="newHireReport">Is this a New Hire or Termination report?</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateEEOHeadcountDetailReport(List<EEOHeadcountDetailReportModel> model, DateTime startDate, DateTime endDate, bool newHireReport, bool openExcel, CancellationToken token);

    /// <summary>
    /// Creates EEO Requisition profile report detailing applicants' EEO statistics for a selected requisition
    /// </summary>
    /// <param name="model">Requisition information</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateEEORequisitionProfileReport(RequisitionProfileReportModel model, bool openExcel, CancellationToken token);

    /// <summary>
    /// Creates EEO summary report for reporting EEO statistics
    /// </summary>
    /// <param name="model">Applicant Lists' EEO information</param>
    /// <param name="startDate">Report start date</param>
    /// <param name="endDate">Report end date</param>
    /// <param name="newHireReport">Is this a New Hire or Termination report?</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateEEOHeadcountSummaryReport(List<EEOHeadcountDetailReportModel> model, DateTime startDate, DateTime endDate, bool newHireReport, bool openExcel, CancellationToken token);

    #endregion

    #region Job Postings

    /// <summary>
    /// Creates a Vacancy Posting for internal or external job openings
    /// </summary>
    /// <param name="model">Requisition information</param>
    /// <param name="jobCode">Job code for the job</param>
    /// <param name="deadlineDate">Date the final application will be accepted</param>
    /// <param name="hiringSupervisor">Hiring supervisor(s) for this requisition</param>
    /// <param name="openExcel">Open worksheet upon creation</param>
    /// <param name="token">Cancellation token</param>
    void CreateVacancyPosting(RequisitionModel model, JobCodeModel jobCode, DateTime deadlineDate, string hiringSupervisor, bool openExcel, CancellationToken token);

    #endregion
}