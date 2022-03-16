namespace HRSuite.Core.Models;
public class RequisitionProfileReportModel
{
    public RequisitionModel Requisition { get; set; }
    public JobCodeModel JobCodeInfo { get; set; }
    public List<ApplicantRequisitionListModel> Applicants { get; set; }
    public List<CodeModel> CodeInfo { get; set; }
}