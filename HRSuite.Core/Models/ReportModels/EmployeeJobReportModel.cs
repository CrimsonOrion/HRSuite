namespace HRSuite.Core.Models;
public class EmployeeJobReportModel : JobHistoryModel
{
    public EmployeeModel Employee { get; set; }
    public string JobTitle { get; set; }
    public int Department { get; set; }
    public string Description { get; set; }
    public string FullDepartmentDescription => ($"{Department} - {Description}").Trim();
}