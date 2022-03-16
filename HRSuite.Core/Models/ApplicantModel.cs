namespace HRSuite.Core.Models;

public class ApplicantModel
{
    public int Id { get; set; }
    public int RequisitionId { get; set; }
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
    public DateTime ApplicationDate { get; set; }
    public string GeneralStatus { get; set; }
    public DateTime StatusAsOf { get; set; }
    public bool Interviewed { get; set; }
    public string LetterCode { get; set; }
    public DateTime LetterSendDate { get; set; }
    public DateTime AcceptDate { get; set; }
    public string RejectionCode { get; set; }
    public string Comment { get; set; }

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