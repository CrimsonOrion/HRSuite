using HRSuite.Core.Models;

using Library.NET.DataAccess;
using Library.NET.DataAccess.ConnectionStrings;

namespace HRSuite.Core.Processors;
public class DataProcessor : IDataProcessor
{
    private readonly ISqlDataAccess _sqlDataAccess;
    private readonly string _connString = MsSqlConnectionStrings.ConnectionString(MsSqlConnection.Home);
    private readonly DateTime _blankDate = new(1900, 1, 1);

    #region Constructor

    public DataProcessor(ISqlDataAccess sqlDataAccess) => _sqlDataAccess = sqlDataAccess;

    #endregion

    #region Applicant

    public async Task<int> CreateApplicantAsync(ApplicantModel model)
    {
        var storedProcedure = "spApplicants_Insert";
        Dictionary<string, object> parameters = new()
        {
            { "@Id", model.Id },
            { "@RequisitionId", model.RequisitionId },
            { "@FirstName", model.FirstName },
            { "@MiddleName", model.MiddleName },
            { "@LastName", model.LastName },
            { "@Nickname", model.Nickname },
            { "@Address1", model.Address1 },
            { "@Address2", model.Address2 },
            { "@City", model.City },
            { "@State", model.State },
            { "@Zip", model.Zip },
            { "@HomePhone", model.HomePhoneNumber },
            { "@CellPhone", model.CellPhoneNumber },
            { "@Sex", model.Sex },
            { "@Ethnicity", model.Ethnicity },
            { "@Disabled", model.Disabled },
            { "@ProtectedVet", model.ProtectedVet },
            { "@DisabledVet", model.DisabledVet },
            { "@ApplicationDate", model.ApplicationDate },
            { "@GeneralStatus", model.GeneralStatus },
            { "@StatusAsOf", model.StatusAsOf },
            { "@Interviewed", model.Interviewed },
            { "@LetterCode", model.LetterCode },
            { "@LetterSendDate", model.LetterSendDate },
            { "@AcceptDate", model.AcceptDate },
            { "@RejectionCode", model.RejectionCode },
            { "@Comment", model.Comment }
        };
        var returnValue = await _sqlDataAccess.PostDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<int> DeleteApplicantByIdAsync(int applicantId)
    {
        var storedProcedure = "spApplicants_DeleteById";
        Dictionary<string, object> parameters = new() { { "@Id", applicantId } };
        var returnValue = await _sqlDataAccess.DeleteDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<ApplicantModel> GetApplicantByIdAsync(int applicantId)
    {
        var storedProcedure = "spApplicants_GetById";
        Dictionary<string, object> parameters = new() { { "@Id", applicantId } };
        IEnumerable<ApplicantModel> returnValue = await _sqlDataAccess.GetDataAsync<ApplicantModel, IDictionary<string, object>>(storedProcedure, _connString, parameters, true);
        return returnValue is null ? new ApplicantModel() : returnValue.FirstOrDefault();
    }

    public async Task<IEnumerable<ApplicantRequisitionListModel>> GetApplicantByRequisitionIdAsync(int requisitionId)
    {
        var storedProcedure = "spApplicants_GetByRequisitionId";
        Dictionary<string, object> parameters = new() { { "@RequisitionId", requisitionId } };
        IEnumerable<ApplicantRequisitionListModel> list = await _sqlDataAccess.GetDataAsync<ApplicantRequisitionListModel, IDictionary<string, object>>(storedProcedure, _connString, parameters, true);
        return list;
    }

    public async Task<int> GetNewApplicantIdAsync()
    {
        var storedProcedure = "spApplicants_GetMaxId";
        IEnumerable<int> newId = await _sqlDataAccess.GetDataAsync<int>(storedProcedure, _connString, true);
        return newId.FirstOrDefault() + 1;
    }

    public async Task<int> UpdateApplicantAsync(ApplicantModel model)
    {
        var storedProcedure = "spApplicants_UpdateById";
        Dictionary<string, object> parameters = new()
        {
            { "@Id", model.Id },
            { "@FirstName", model.FirstName },
            { "@MiddleName", model.MiddleName },
            { "@LastName", model.LastName },
            { "@Nickname", model.Nickname },
            { "@Address1", model.Address1 },
            { "@Address2", model.Address2 },
            { "@City", model.City },
            { "@State", model.State },
            { "@Zip", model.Zip },
            { "@HomePhone", model.HomePhoneNumber },
            { "@CellPhone", model.CellPhoneNumber },
            { "@Sex", model.Sex },
            { "@Ethnicity", model.Ethnicity },
            { "@Disabled", model.Disabled },
            { "@ProtectedVet", model.ProtectedVet },
            { "@DisabledVet", model.DisabledVet },
            { "@ApplicationDate", model.ApplicationDate },
            { "@GeneralStatus", model.GeneralStatus },
            { "@StatusAsOf", model.StatusAsOf },
            { "@Interviewed", model.Interviewed },
            { "@LetterCode", model.LetterCode },
            { "@LetterSendDate", model.LetterSendDate },
            { "@AcceptDate", model.AcceptDate },
            { "@RejectionCode", model.RejectionCode },
            { "@Comment", model.Comment }
        };
        var returnValue = await _sqlDataAccess.PutDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    #endregion

    #region Code

    public async Task<int> UpdateCodeByIdAsync(CodeModel model)
    {
        var storedProcedure = "spCodes_UpdateById";
        Dictionary<string, object> parameters = new() { { "@Id", model.Id }, { "@Type", model.Type }, { "@Code", model.Code }, { "@Description", model.Description } };
        var returnValue = await _sqlDataAccess.PutDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<int> CreateCodeAsync(CodeModel model)
    {
        var storedProcedure = "spCodes_Insert";
        Dictionary<string, object> parameters = new() { { "@Type", model.Type }, { "@Code", model.Code }, { "@Description", model.Description } };
        var returnValue = await _sqlDataAccess.PostDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<int> DeleteCodeByIdAsync(CodeModel model)
    {
        var storedProcedure = "spCodes_DeleteById";
        Dictionary<string, object> parameters = new() { { "@Id", model.Id } };
        var returnValue = await _sqlDataAccess.DeleteDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<IEnumerable<CodeModel>> GetCodesByType(string type)
    {
        var storedProcedure = "spCodes_GetByType";
        Dictionary<string, object> parameters = new() { { "@type", type } };
        IEnumerable<CodeModel> list = await _sqlDataAccess.GetDataAsync<CodeModel, IDictionary<string, object>>(storedProcedure, _connString, parameters, true);
        return list;
    }

    public async Task<IEnumerable<CodeModel>> GetAllCodesAsync()
    {
        var storedProcedure = "spCodes_GetAll";
        IEnumerable<CodeModel> list = await _sqlDataAccess.GetDataAsync<CodeModel>(storedProcedure, _connString, true);
        return list;
    }

    public async Task<IEnumerable<string>> GetTypesAsync()
    {
        var storedProcedure = "spCodes_GetTypes";
        IEnumerable<string> list = await _sqlDataAccess.GetDataAsync<string>(storedProcedure, _connString, true);
        return list;
    }

    #endregion

    #region Employee

    public async Task<int> DeleteEmployeeByIdAsync(int id)
    {
        var storedProcedure = "spEmployees_DeleteById";
        Dictionary<string, object> parameters = new() { { "@Id", id } };
        var returnValue = await _sqlDataAccess.DeleteDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<IEnumerable<EmployeeModel>> GetActiveEmployeeCDLAsync()
    {
        var storedProcedure = "spEmployees_GetActiveCDL";
        IEnumerable<EmployeeModel> returnValue = await _sqlDataAccess.GetDataAsync<EmployeeModel>(storedProcedure, _connString, true);
        return returnValue;
    }

    public async Task<IEnumerable<EmployeeModel>> GetAllActiveEmployeesAsync()
    {
        var storedProcedure = "spEmployees_GetAllActive";
        IEnumerable<EmployeeModel> returnValue = await _sqlDataAccess.GetDataAsync<EmployeeModel>(storedProcedure, _connString, true);
        return returnValue;
    }

    public async Task<IEnumerable<EmployeeModel>> GetAllEmployeesAsync()
    {
        var storedProcedure = "spEmployees_GetAll";
        IEnumerable<EmployeeModel> returnValue = await _sqlDataAccess.GetDataAsync<EmployeeModel>(storedProcedure, _connString, true);
        return returnValue;
    }

    public async Task<int> CreateEmployeeAsync(ApplicantModel model)
    {
        var storedProcedure = "spEmployees_Insert";
        var newEmployeeId = await GetNewEmployeeIdAsync();
        Dictionary<string, object> parameters = new()
        {
            { "@Id", newEmployeeId },
            { "@FirstName", model.FirstName },
            { "@MiddleName", model.MiddleName },
            { "@LastName", model.LastName },
            { "@Nickname", model.Nickname },
            { "@Address1", model.Address1 },
            { "@Address2", model.Address2 },
            { "@City", model.City },
            { "@State", model.State },
            { "@Zip", model.Zip },
            { "@HomePhone", model.HomePhoneNumber },
            { "@CellPhone", model.CellPhoneNumber },
            { "@Sex", model.Sex },
            { "@Ethnicity", model.Ethnicity },
            { "@Disabled", model.Disabled },
            { "@ProtectedVet", model.ProtectedVet },
            { "@DisabledVet", model.DisabledVet },
            { "@Birthday", _blankDate },
            { "@Active", true },
            { "@BusinessExt", "" },
            { "@EmployeeEmail", "" },
            { "@OriginalHireDate", DateTime.Today },
            { "@SeniorityDate", DateTime.Today },
            { "@TerminationDate", _blankDate },
            { "@TerminationCode", "" },
            { "@Rehired", false },
            { "@LicenseNumber", "" },
            { "@LicenseExp", _blankDate },
            { "@CDL_NonCDL", "" },
            { "@CDLMedExp", _blankDate },
            { "@CDLDriverType", "" },
            { "@LicenseNotes", "" }
        };
        _ = await _sqlDataAccess.PostDataAsync(storedProcedure, _connString, parameters, true);
        return newEmployeeId;
    }

    public async Task<int> UpdateEmployeeActiveStatusAsync(int id, bool active, string terminationCode, DateTime terminationDate)
    {
        if (active)
        {
            terminationDate = new DateTime(1900, 1, 1);
            terminationCode = "";
        }
        var storedProcedure = "spEmployees_UpdateActiveStatusById";
        Dictionary<string, object> parameters = new() { { "@Id", id }, { "@Active", active }, { "@TerminationDate", terminationDate }, { "@TerminationCode", terminationCode } };
        var returnValue = await _sqlDataAccess.PutDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<int> UpdateEmployeeByIdAsync(EmployeeModel model)
    {
        var storedProcedure = "spEmployees_UpdateById";
        Dictionary<string, object> parameters = new()
        {
            { "@Id", model.Id },
            { "@FirstName", model.FirstName },
            { "@MiddleName", model.MiddleName },
            { "@LastName", model.LastName },
            { "@Nickname", model.Nickname },
            { "@Address1", model.Address1 },
            { "@Address2", model.Address2 },
            { "@City", model.City },
            { "@State", model.State },
            { "@Zip", model.Zip },
            { "@HomePhone", model.HomePhoneNumber },
            { "@CellPhone", model.CellPhoneNumber },
            { "@Sex", model.Sex },
            { "@Ethnicity", model.Ethnicity },
            { "@Disabled", model.Disabled },
            { "@ProtectedVet", model.ProtectedVet },
            { "@DisabledVet", model.DisabledVet },
            { "@Birthday", model.Birthday },
            { "@Active", model.Active },
            { "@BusinessExt", model.BusinessExt },
            { "@EmployeeEmail", model.EmployeeEmail },
            { "@OriginalHireDate", model.OriginalHireDate },
            { "@SeniorityDate", model.SeniorityDate },
            { "@TerminationDate", model.TerminationDate },
            { "@TerminationCode", model.TerminationCode },
            { "@Rehired", model.Rehired },
            { "@LicenseNumber", model.LicenseNumber },
            { "@LicenseExp", model.LicenseExp },
            { "@CDL_NonCDL", model.CDL_NonCDL },
            { "@CDLMedExp", model.CDLMedExp },
            { "@CDLDriverType", model.CDLDriverType },
            { "@LicenseNotes", model.LicenseNotes }
        };
        var returnValue = await _sqlDataAccess.PutDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    private async Task<int> GetNewEmployeeIdAsync()
    {
        var storedProcedure = "spEmployees_GetMaxId";
        IEnumerable<int> newId = await _sqlDataAccess.GetDataAsync<int>(storedProcedure, _connString, true);
        return newId.FirstOrDefault() + 1;
    }

    #endregion

    #region InternalApplicants

    public async Task<InternalApplicantModel> GetInternalApplicantByApplicantIdAsync(int applicantId)
    {
        var storedProcedure = "spInternalApplicants_GetByApplicantId";
        Dictionary<string, object> parameters = new() { { "@ApplicantId", applicantId } };
        IEnumerable<InternalApplicantModel> returnValue = await _sqlDataAccess.GetDataAsync<InternalApplicantModel, IDictionary<string, object>>(storedProcedure, _connString, parameters, true);
        return returnValue is null ? new InternalApplicantModel() : returnValue.FirstOrDefault();
    }

    public async Task<int> DeleteInternalApplicantByApplicantIdAsync(int applicantId)
    {
        var storedProcedure = "spInternalApplicants_DeleteByApplicantId";
        Dictionary<string, object> parameters = new() { { "@ApplicantId", applicantId } };
        var returnValue = await _sqlDataAccess.DeleteDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<int> CreateInternalApplicantAsync(InternalApplicantModel model)
    {
        var storedProcedure = "spInternalApplicants_Insert";
        Dictionary<string, object> parameters = new() { { "@EmployeeId", model.EmployeeId }, { "@ApplicantId", model.ApplicantId } };
        var returnValue = await _sqlDataAccess.PostDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    #endregion

    #region Job Codes

    public async Task<int> DeleteJobCodeByIdAsync(JobCodeModel model)
    {
        var storedProcedure = "spJobCodes_DeleteById";
        Dictionary<string, object> parameters = new() { { "@Id", model.Id } };
        var returnValue = await _sqlDataAccess.DeleteDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<IEnumerable<JobCodeModel>> GetAllJobCodesAsync()
    {
        var storedProcedure = "spJobCodes_GetAll";
        IEnumerable<JobCodeModel> list = await _sqlDataAccess.GetDataAsync<JobCodeModel>(storedProcedure, _connString, true);
        return list;
    }

    public async Task<int> CreateJobCodeAsync(JobCodeModel model)
    {
        var storedProcedure = "spJobCodes_Insert";
        Dictionary<string, object> parameters = new()
        {
            { "@Code", model.Code },
            { "@EEOClass", model.EEOClass },
            { "@Department", model.Department },
            { "@JobTitle", model.JobTitle },
            { "@InsideOutside", model.InsideOutside },
            { "@SupervisorCode", model.SupervisorCode },
            { "@RequisitionType", model.RequisitionType },
            { "@Exempt", model.Exempt }
        };
        var returnValue = await _sqlDataAccess.PostDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<int> UpdateJobCodeByIdAsync(JobCodeModel model)
    {
        var storedProcedure = "spJobCodes_UpdateById";
        Dictionary<string, object> parameters = new()
        {
            { "@Id", model.Id },
            { "@Code", model.Code },
            { "@EEOClass", model.EEOClass },
            { "@Department", model.Department },
            { "@JobTitle", model.JobTitle },
            { "@InsideOutside", model.InsideOutside },
            { "@SupervisorCode", model.SupervisorCode },
            { "@RequisitionType", model.RequisitionType },
            { "@Exempt", model.Exempt }
        };
        var returnValue = await _sqlDataAccess.PutDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    #endregion

    #region Job History

    public async Task<int> DeleteJobHistoryAsync(JobHistoryModel model)
    {
        var storedProcedure = "spJobHistory_DeleteById";
        IEnumerable<EmployeeJobReportModel> allJobHistory = await GetAllJobHistoryAsync();
        var id = allJobHistory.LastOrDefault(_ => _.EmployeeId == model.EmployeeId && _.JobCode == model.JobCode).Id;
        Dictionary<string, object> parameters = new()
        {
            { "@Id", id }
        };
        var returnValue = await _sqlDataAccess.DeleteDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<IEnumerable<EmployeeJobHistoryModel>> GetJobHistoryByEmployeeIdAsync(int empId)
    {
        var storedProcedure = "spJobHistory_GetByEmployeeId";
        Dictionary<string, object> parameters = new() { { "@EmployeeId", empId } };
        IEnumerable<EmployeeJobHistoryModel> returnValue = await _sqlDataAccess.GetDataAsync<EmployeeJobHistoryModel, IDictionary<string, object>>(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<IEnumerable<JobHistoryModel>> GetJobHistoryByJobCodeAsync(string jobCode)
    {
        var storedProcedure = "spJobHistory_GetByJobCode";
        Dictionary<string, object> parameters = new() { { "@Code", jobCode } };
        IEnumerable<JobHistoryModel> returnValue = await _sqlDataAccess.GetDataAsync<JobHistoryModel, IDictionary<string, object>>(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<int> CreateEmployeeJobHistoryAsync(JobHistoryModel model)
    {
        var storedProcedure = "spJobHistory_Insert";
        Dictionary<string, object> parameters = new()
        {
            { "@EmployeeId", model.EmployeeId },
            { "@JobCode", model.JobCode },
            { "@ChangeDate", model.ChangeDate },
            { "@JobDate", model.JobDate },
            { "@Reason", model.Reason }
        };
        var newId = await _sqlDataAccess.PostDataAsync(storedProcedure, _connString, parameters, true);
        return newId;
    }

    private async Task<IEnumerable<EmployeeJobReportModel>> GetAllJobHistoryAsync()
    {
        var storedProcedure = "spJobHistory_GetAll";
        IEnumerable<EmployeeJobReportModel> returnValue = await _sqlDataAccess.GetDataAsync<EmployeeJobReportModel>(storedProcedure, _connString, true);
        return returnValue;
    }

    #endregion

    #region Requisition

    public async Task<int> CreateRequisitionAsync(RequisitionModel model)
    {
        var storedProcedure = "spRequisitions_Insert";
        Dictionary<string, object> parameters = new()
        {
            { "@Id", model.Id },
            { "@Code", model.Code },
            { "@Description", model.Description },
            { "@Status", model.Status },
            { "@JobCode", model.JobCode },
            { "@Internal", model.Internal },
            { "@NumPos", model.NumPos },
            { "@ReasonOpen", model.ReasonOpen },
            { "@ReasonClosed", model.ReasonClosed },
            { "@CreateDate", model.CreateDate },
            { "@LastChangeDate", model.LastChangeDate },
            { "@OpenDate", model.OpenDate },
            { "@CloseDate", model.CloseDate },
            { "@Comment", model.Comment }
        };
        var returnValue = await _sqlDataAccess.PostDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<int> DeleteRequisitionByIdAsync(int requisitionId)
    {
        var storedProcedure = "spRequisitions_DeleteById";
        Dictionary<string, object> parameters = new() { { "@Id", requisitionId } };
        var returnValue = await _sqlDataAccess.DeleteDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    public async Task<IEnumerable<RequisitionModel>> GetAllRequisitionsAsync()
    {
        var storedProcedure = "spRequisitions_GetAll";
        IEnumerable<RequisitionModel> list = await _sqlDataAccess.GetDataAsync<RequisitionModel>(storedProcedure, _connString, true);
        return list;
    }

    public async Task<int> GetNewRequisitionIdAsync()
    {
        var storedProcedure = "spRequisitions_GetMaxId";
        IEnumerable<int> newId = await _sqlDataAccess.GetDataAsync<int>(storedProcedure, _connString, true);
        return newId.FirstOrDefault() + 1;
    }

    public async Task<int> UpdateRequisitionAsync(RequisitionModel model)
    {
        var storedProcedure = "spRequisitions_Update";
        Dictionary<string, object> parameters = new()
        {
            { "@Id", model.Id },
            { "@Code", model.Code },
            { "@Description", model.Description },
            { "@Status", model.Status },
            { "@JobCode", model.JobCode },
            { "@Internal", model.Internal },
            { "@NumPos", model.NumPos },
            { "@ReasonOpen", model.ReasonOpen },
            { "@ReasonClosed", model.ReasonClosed },
            { "@CreateDate", model.CreateDate },
            { "@LastChangeDate", model.LastChangeDate },
            { "@OpenDate", model.OpenDate },
            { "@CloseDate", model.CloseDate },
            { "@Comment", model.Comment }
        };
        var returnValue = await _sqlDataAccess.PutDataAsync(storedProcedure, _connString, parameters, true);
        return returnValue;
    }

    #endregion

    #region Reporting

    public async Task<IEnumerable<EmployeeJobReportModel>> GetAgeListAsync()
    {
        IEnumerable<EmployeeModel> activeEmployees = await GetAllActiveEmployeesAsync();
        IEnumerable<EmployeeJobReportModel> allJobHistory = await GetAllJobHistoryAsync();
        List<EmployeeJobReportModel> list = new();

        foreach (EmployeeModel employee in activeEmployees)
        {
            EmployeeJobReportModel model = new();
            model.Employee = employee;
            EmployeeJobReportModel jobInfo = allJobHistory.FirstOrDefault(_ => _.EmployeeId == employee.Id);
            model.JobTitle = jobInfo.JobTitle;
            model.Department = jobInfo.Department;
            model.Description = jobInfo.Description;
            list.Add(model);
        }

        return list.OrderBy(_ => _.Employee.Birthday);
    }

    public async Task<IEnumerable<PromotionReportModel>> GetPromotionsByDateAsync(DateTime startDate, DateTime endDate)
    {
        var storedProcedure = "spJobHistory_GetPromotionsByDate";
        Dictionary<string, object> parameters = new() { { "@startDate", startDate }, { "@endDate", endDate } };
        IEnumerable<PromotionReportModel> list = await _sqlDataAccess.GetDataAsync<PromotionReportModel, IDictionary<string, object>>(storedProcedure, _connString, parameters, true);
        return list;
    }

    public async Task<IEnumerable<EEOHeadcountDetailReportModel>> GetNewHireHeadcount(DateTime startDate, DateTime endDate)
    {
        var storedProcedure = "spEmployees_GetNewHireInfoByDate";
        Dictionary<string, object> parameters = new() { { "@startDate", startDate }, { "@endDate", endDate } };
        IEnumerable<EEOHeadcountDetailReportModel> list = await _sqlDataAccess.GetDataAsync<EEOHeadcountDetailReportModel, IDictionary<string, object>>(storedProcedure, _connString, parameters, true);
        IEnumerable<CodeModel> codes = await GetAllCodesAsync();
        foreach (EEOHeadcountDetailReportModel employee in list)
        {
            employee.DepartmentDescription = codes.FirstOrDefault(_ => _.Type == "DEPT" && _.Code == $"{employee.Department}").Description;
            employee.EEODescription = codes.FirstOrDefault(_ => _.Type == "EEO" && _.Code == employee.EEOClass).Description;

            CodeModel sexCode = codes.FirstOrDefault(_ => _.Type == "GENDER" && _.Code == employee.Sex);
            employee.Sex = sexCode is null ? "Undeclared" : sexCode.Description;

            CodeModel ethnicCode = codes.FirstOrDefault(_ => _.Type == "ETHNIC" && _.Code == employee.Ethnicity);
            employee.Ethnicity = ethnicCode is null ? "Undeclared" : ethnicCode.Description;
        }

        return list;
    }

    public async Task<IEnumerable<EEOHeadcountDetailReportModel>> GetTerminatedHeadcount(DateTime startDate, DateTime endDate)
    {

        var storedProcedure = "spEmployees_GetTerminatedInfoByDate";
        Dictionary<string, object> parameters = new() { { "@startDate", startDate }, { "@endDate", endDate } };
        IEnumerable<EmployeeModel> employees = await _sqlDataAccess.GetDataAsync<EmployeeModel, IDictionary<string, object>>(storedProcedure, _connString, parameters, true);
        IEnumerable<CodeModel> codes = await GetAllCodesAsync();
        IEnumerable<JobCodeModel> jobCodes = await Task.Run(() => GetAllJobCodesAsync());
        IEnumerable<EmployeeJobReportModel> jobHistory = await GetAllJobHistoryAsync();

        List<EEOHeadcountDetailReportModel> list = new();

        foreach (EmployeeModel employee in employees)
        {
            EmployeeJobReportModel currentJob = jobHistory.Where(_ => _.EmployeeId == employee.Id).OrderByDescending(_ => _.JobDate).FirstOrDefault();
            var eeoClass = jobCodes.FirstOrDefault(_ => _.Code == currentJob.JobCode).EEOClass;
            CodeModel sexCode = codes.FirstOrDefault(_ => _.Type == "GENDER" && _.Code == employee.Sex);
            CodeModel ethnicCode = codes.FirstOrDefault(_ => _.Type == "ETHNIC" && _.Code == employee.Ethnicity);
            EEOHeadcountDetailReportModel model = new()
            {
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                LastName = employee.LastName,
                JobTitle = currentJob.JobTitle,
                Department = currentJob.Department,
                DepartmentDescription = currentJob.Description,
                EEOClass = eeoClass,
                EEODescription = codes.FirstOrDefault(_ => _.Type == "EEO" && _.Code == eeoClass).Description,
                SeniorityDate = employee.TerminationDate,
                Disabled = employee.Disabled,
                DisabledVet = employee.DisabledVet,
                ProtectedVet = employee.ProtectedVet,
                Sex = sexCode is null ? "Undeclared" : sexCode.Description,
                Ethnicity = ethnicCode is null ? "Undeclared" : ethnicCode.Description
            };
            list.Add(model);
        }

        return list;
    }

    #endregion
}