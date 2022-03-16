namespace HRSuite.Core.Models;

public class ApplicantRequisitionListModel
{
    public int ApplicantId { get; set; }
    public int RequisitionId { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Code { get; set; }
    public string Sex { get; set; }
    public string Ethnicity { get; set; }
    public string Disabled { get; set; }
    public string ProtectedVet { get; set; }
    public string DisabledVet { get; set; }
    public string Interviewed { get; set; }
    public string GeneralStatus { get; set; }
    public DateTime ApplicationDate { get; set; }
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
