namespace HRSuite.Core.Models;

public class RequisitionModel
{
    public int Id { get; set; }
    public string Code => $"{CreateDate:yyyyMMdd}-{JobCode}-{NumPos}{(Internal ? "I" : "")}";
    public string Description { get; set; }
    public string Status { get; set; }
    public string JobCode { get; set; }
    public bool Internal { get; set; }
    public int NumPos { get; set; }
    public string ReasonOpen { get; set; }
    public string ReasonClosed { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastChangeDate { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime CloseDate { get; set; }
    public string Comment { get; set; }
    public List<ApplicantModel> Hired { get; set; }
}
