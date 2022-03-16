namespace HRSuite.Core.Models;
public class EEOHeadcountDetailReportModel
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string JobTitle { get; set; }
    public int Department { get; set; }
    public string DepartmentDescription { get; set; }
    public string EEOClass { get; set; }
    public string EEODescription { get; set; }
    public DateTime SeniorityDate { get; set; }
    public string Disabled { get; set; }
    public string DisabledVet { get; set; }
    public string ProtectedVet { get; set; }
    public string Sex { get; set; }
    public string Ethnicity { get; set; }

    public string FullName
    {
        get
        {
            var middleName = !string.IsNullOrEmpty(MiddleName) ? MiddleName : "";
            var name = $"{LastName}, {FirstName} {middleName}";
            return name.Trim();
        }
    }
    public string JobTitleDepartment => $"{JobTitle.Trim()} - {DepartmentDescription.Trim()}";
    public string EEOClassDescription => $"{EEOClass.Trim()} - {EEODescription.Trim()}";
    public DateTime EffectiveDate => SeniorityDate;
}