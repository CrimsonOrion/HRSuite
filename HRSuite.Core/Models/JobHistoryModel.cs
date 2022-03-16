namespace HRSuite.Core.Models;

public class JobHistoryModel
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string JobCode { get; set; }
    public DateTime ChangeDate { get; set; }
    public DateTime JobDate { get; set; }
    public string Reason { get; set; }
}