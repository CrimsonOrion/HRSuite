namespace HRSuite.Core.Models;

public class CodeModel
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string FullDescription => $"{Code} - {Description}";
}
