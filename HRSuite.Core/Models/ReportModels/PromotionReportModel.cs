namespace HRSuite.Core.Models;
public class PromotionReportModel
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string EEOClass { get; set; }
    public string Sex { get; set; }
    public string Ethnicity { get; set; }
    public string Department { get; set; }
    public string Description { get; set; }
    public string JobTitle { get; set; }
    public string Reason { get; set; }
    public string FullDescription => ($"{Department} - {Description}").Trim();
    public string FullName
    {
        get
        {
            var middleName = !string.IsNullOrEmpty(MiddleName) ? MiddleName : "";
            var name = $"{LastName}, {FirstName} {middleName}";
            return name.Trim();
        }
    }
}