using HRSuite.Core.Models;

namespace HRSuite.Core.Processors;

public interface IDataProcessor
{
    #region Applicant

    /// <summary>
    /// Insert Applicant into database
    /// </summary>
    /// <param name="model">ApplicantModel with applicant's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> CreateApplicantAsync(ApplicantModel model);

    /// <summary>
    /// Delete Applicant from database by Id
    /// </summary>
    /// <param name="applicantId">Applicant ID value</param>
    /// <returns>Number of affected rows</returns>
    Task<int> DeleteApplicantByIdAsync(int applicantId);

    /// <summary>
    /// Retrieve applicant information by Id
    /// </summary>
    /// <param name="applicantId">Applicant Id</param>
    /// <returns>ApplicantModel information</returns>
    Task<ApplicantModel> GetApplicantByIdAsync(int applicantId);

    /// <summary>
    /// Retrieve list of applicants by requisition Id
    /// </summary>
    /// <param name="requisitionId">Requisition Id</param>
    /// <returns>List of applicants for the given requisition</returns>
    Task<IEnumerable<ApplicantRequisitionListModel>> GetApplicantByRequisitionIdAsync(int requisitionId);

    /// <summary>
    /// Find and return one higher than MAX(id) in the applicants table
    /// </summary>
    /// <returns>New Applicant Id</returns>
    Task<int> GetNewApplicantIdAsync();

    /// <summary>
    /// Update applicant information in database
    /// </summary>
    /// <param name="model">ApplicantModel with applicant's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> UpdateApplicantAsync(ApplicantModel model);

    #endregion

    #region Code

    /// <summary>
    /// Insert Code into database
    /// </summary>
    /// <param name="model">CodeModel with code's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> CreateCodeAsync(CodeModel model);

    /// <summary>
    /// Delete Code from database by Id
    /// </summary>
    /// <param name="model">CodeModel with code's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> DeleteCodeByIdAsync(CodeModel model);

    /// <summary>
    /// Retrieve all codes
    /// </summary>
    /// <returns>List of all [Code]s in Codes table</returns>
    Task<IEnumerable<CodeModel>> GetAllCodesAsync();

    /// <summary>
    /// Retrieve all codes by type
    /// </summary>
    /// <param name="type">Type to retrieve codes by</param>
    /// <returns>CodeModels by given type</returns>
    Task<IEnumerable<CodeModel>> GetCodesByType(string type);

    /// <summary>
    /// Retrieve all types
    /// </summary>
    /// <returns>List of all [Type]s in Codes table</returns>
    Task<IEnumerable<string>> GetTypesAsync();

    /// <summary>
    /// Update code information in database
    /// </summary>
    /// <param name="model">CodeModel with code's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> UpdateCodeByIdAsync(CodeModel model);

    #endregion

    #region Employee

    /// <summary>
    /// Insert Employee into database
    /// </summary>
    /// <param name="model">ApplicantModel with new employee</param>
    /// <returns>New EmployeeID</returns>
    Task<int> CreateEmployeeAsync(ApplicantModel model);

    /// <summary>
    /// Delete Employee from database by Id
    /// </summary>
    /// <param name="id">Employee Id for Employee to delete</param>
    /// <returns>Number of affected rows</returns>
    Task<int> DeleteEmployeeByIdAsync(int id);

    /// <summary>
    /// Retrieve Active Employees' CDL information
    /// </summary>
    /// <returns>List of employees' CDL information</returns>
    Task<IEnumerable<EmployeeModel>> GetActiveEmployeeCDLAsync();

    /// <summary>
    /// Retrieve all active employees
    /// </summary>
    /// <returns>List of all active employees in Employees table</returns>
    Task<IEnumerable<EmployeeModel>> GetAllActiveEmployeesAsync();

    /// <summary>
    /// Retrieve all employees
    /// </summary>
    /// <returns>List of all employees in Employees table</returns>
    Task<IEnumerable<EmployeeModel>> GetAllEmployeesAsync();

    /// <summary>
    /// Update employee active status by Id, populating termination code and date
    /// </summary>
    /// <param name="id">Employee Id</param>
    /// <param name="active">Active status</param>
    /// <param name="terminationCode">Termination Code</param>
    /// <param name="terminationDate">Termination Date</param>
    /// <returns>Number of affected rows</returns>
    Task<int> UpdateEmployeeActiveStatusAsync(int id, bool active, string terminationCode, DateTime terminationDate);

    /// <summary>
    /// Update employee by Id
    /// </summary>
    /// <param name="model">EmployeeModel with employee's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> UpdateEmployeeByIdAsync(EmployeeModel model);

    #endregion

    #region Internal Applicants

    /// <summary>
    /// Insert Internal Applicant into database
    /// </summary>
    /// <param name="model">InternalApplicantModel with applicant's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> CreateInternalApplicantAsync(InternalApplicantModel model);

    /// <summary>
    /// Delete Internal Applicant from database by Id
    /// </summary>
    /// <param name="applicantId">Internal Applicant's Id</param>
    /// <returns>Number of affected rows</returns>
    Task<int> DeleteInternalApplicantByApplicantIdAsync(int applicantId);

    /// <summary>
    /// Retrieve Internal Applicant by Id
    /// </summary>
    /// <param name="applicantId">Internal Applicant's Id</param>
    /// <returns>InternalApplicantModel of internal applicant's information</returns>
    Task<InternalApplicantModel> GetInternalApplicantByApplicantIdAsync(int applicantId);

    #endregion

    #region Job Codes

    /// <summary>
    /// Insert Job Code into database
    /// </summary>
    /// <param name="model">JobCodeModel with job code's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> CreateJobCodeAsync(JobCodeModel model);

    /// <summary>
    /// Delete Job Code from database
    /// </summary>
    /// <param name="model">JobCodeModel with job code's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> DeleteJobCodeByIdAsync(JobCodeModel model);

    /// <summary>
    /// Retrieve list of all job codes
    /// </summary>
    /// <returns>List of JobCodeModels from JobCode table</returns>
    Task<IEnumerable<JobCodeModel>> GetAllJobCodesAsync();

    /// <summary>
    /// Update job code by Id
    /// </summary>
    /// <param name="model">JobCodeModel with job code's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> UpdateJobCodeByIdAsync(JobCodeModel model);

    #endregion

    #region Job History

    /// <summary>
    /// Insert Job History entry for an employee into database
    /// </summary>
    /// <param name="model">JobHistoryModel with job history's information</param>
    /// <returns>New JobHistory Id</returns>
    Task<int> CreateEmployeeJobHistoryAsync(JobHistoryModel model);

    /// <summary>
    /// Delete Job History entry for an employee from database
    /// </summary>
    /// <param name="model">JobHistoryModel with job history's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> DeleteJobHistoryAsync(JobHistoryModel model);

    /// <summary>
    /// Retrieve Job History entry for an employee by employee Id
    /// </summary>
    /// <param name="empId">Employee Id</param>
    /// <returns>List of Job History for given employee Id</returns>
    Task<IEnumerable<EmployeeJobHistoryModel>> GetJobHistoryByEmployeeIdAsync(int empId);

    /// <summary>
    /// Retrieve Job History entries by Job Code
    /// </summary>
    /// <param name="jobCode">Job Code</param>
    /// <returns>Lost of Job History for given job code</returns>
    Task<IEnumerable<JobHistoryModel>> GetJobHistoryByJobCodeAsync(string jobCode);

    #endregion

    #region Requisition

    /// <summary>
    /// Insert Requisition into database
    /// </summary>
    /// <param name="model">RequisitionModel with requisition's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> CreateRequisitionAsync(RequisitionModel model);

    /// <summary>
    /// Delete Requisition from database
    /// </summary>
    /// <param name="requisitionId">Requisition Id</param>
    /// <returns>Number of affected rows</returns>
    Task<int> DeleteRequisitionByIdAsync(int requisitionId);

    /// <summary>
    /// Retrieve all requisitions
    /// </summary>
    /// <returns>List of all requisitions in Requisitions table</returns>
    Task<IEnumerable<RequisitionModel>> GetAllRequisitionsAsync();

    /// <summary>
    /// Find and return one higher than MAX(id) in the requisitions table
    /// </summary>
    /// <returns>New Requisition Id</returns>
    Task<int> GetNewRequisitionIdAsync();

    /// <summary>
    /// Update requisition
    /// </summary>
    /// <param name="model">RequisitionModel with requisition's information</param>
    /// <returns>Number of affected rows</returns>
    Task<int> UpdateRequisitionAsync(RequisitionModel model);

    #endregion

    #region Reporting

    /// <summary>
    /// Retrieve employee information for Age List Report
    /// </summary>
    /// <returns>List of EmployeeJobReportModels sorted by age</returns>
    Task<IEnumerable<EmployeeJobReportModel>> GetAgeListAsync();

    /// <summary>
    /// Retrieve New Hire EEO information for New Hire EEO Report
    /// </summary>
    /// <param name="startDate">Start date for report</param>
    /// <param name="endDate">End date for report</param>
    /// <returns>List of EEOHeadcountDetailReportModels sorted by EEO Class</returns>
    Task<IEnumerable<EEOHeadcountDetailReportModel>> GetNewHireHeadcount(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Retrieve Promotion information between two dates
    /// </summary>
    /// <param name="startDate">Start date for report</param>
    /// <param name="endDate">End date for report</param>
    /// <returns>List of PromotionReportModels sorted by date</returns>
    Task<IEnumerable<PromotionReportModel>> GetPromotionsByDateAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Retrieve Terminated EEO information for Terminated EEO Report
    /// </summary>
    /// <param name="startDate">Start date for report</param>
    /// <param name="endDate">End date for report</param>
    /// <returns>List of EEOHeadcountDetailReportModels sorted by EEO Class</returns>
    Task<IEnumerable<EEOHeadcountDetailReportModel>> GetTerminatedHeadcount(DateTime startDate, DateTime endDate);

    #endregion
}