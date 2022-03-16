namespace HRSuite.Core.Models;

public class JobCodeModel
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string EEOClass { get; set; }
    public string Department { get; set; }
    public string JobTitle { get; set; }
    public string InsideOutside { get; set; }
    public string SupervisorCode { get; set; }
    public string RequisitionType { get; set; }
    public bool Exempt { get; set; }
    public bool Depreciated => !string.IsNullOrEmpty(JobTitle) && JobTitle.Contains("DEPR");
    public string FullDescription => string.IsNullOrEmpty(JobTitle) && string.IsNullOrEmpty(Code) ? string.Empty : $"{Code} - {JobTitle}";
}