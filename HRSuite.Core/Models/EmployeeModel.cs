namespace HRSuite.Core.Models;

public class EmployeeModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Nickname { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string HomePhone { get; set; }
    public string CellPhone { get; set; }
    public string Sex { get; set; }
    public string Ethnicity { get; set; }
    public string Disabled { get; set; }
    public string ProtectedVet { get; set; }
    public string DisabledVet { get; set; }
    public DateTime Birthday { get; set; }
    public bool Active { get; set; }
    public string BusinessPhone { get; set; }
    public string BusinessExt { get; set; }
    public string EmployeeEmail { get; set; }
    public DateTime OriginalHireDate { get; set; }
    public DateTime SeniorityDate { get; set; }
    public DateTime TerminationDate { get; set; }
    public string TerminationCode { get; set; }
    public bool Rehired { get; set; }
    public string LicenseNumber { get; set; }
    public DateTime LicenseExp { get; set; }
    public string CDL_NonCDL { get; set; }
    public DateTime CDLMedExp { get; set; }
    public string CDLDriverType { get; set; }
    public string LicenseNotes { get; set; }
    public JobCodeModel Position { get; set; }
    public List<JobHistoryModel> JobHistory { get; set; }

    public string FullName
    {
        get
        {
            var middleName = !string.IsNullOrEmpty(MiddleName) ? MiddleName : "";
            var name = $"{LastName}, {FirstName} {middleName}";
            return name.Trim();
        }
    }

    public long? HomePhoneNumber
    {
        get
        {
            var hp = HomePhone?.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim();
            return long.TryParse(hp, out var n) ? n : null;
        }
    }

    public long? CellPhoneNumber
    {
        get
        {
            var cp = CellPhone?.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim();
            return long.TryParse(cp, out var n) ? n : null;
        }
    }
}
