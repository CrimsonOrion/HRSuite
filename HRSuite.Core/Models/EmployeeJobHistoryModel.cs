namespace HRSuite.Core.Models;
public class EmployeeJobHistoryModel
{
    public string JobCode { get; set; }
    public string JobTitle { get; set; }
    public DateTime ChangeDate { get; set; }
    public DateTime JobDate { get; set; }
    public string Reason { get; set; }
}