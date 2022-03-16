using System.Diagnostics;

using GemBox.Document;

using HRSuite.Core.ConfigurationModels;
using HRSuite.Core.Models;

using Library.NET.Helpers;

namespace HRSuite.Core.Reporting;
public class Letter : ILetter
{
    #region Constructor 

    public Letter() => ComponentInfo.SetLicense("FREE-LIMITED-KEY");

    #endregion

    #region Rejection Letter

    public void CreateRejectionLetter(ApplicantModel model, string positionTitle, bool openWord, CancellationToken token)
    {
        // Check if user wants to cancel the report generation
        if (token.IsCancellationRequested)
        {
            token.ThrowIfCancellationRequested();
        }

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{model.FirstName.Trim()} {model.LastName.Trim()} {positionTitle.Trim().Replace('/', '-').Replace('\\', '-').ToUpper()}.docx"));

        var template = File.Exists(GlobalConfig.FileLocationsConfiguration.LetterheadBwDfs) ? GlobalConfig.FileLocationsConfiguration.LetterheadBwDfs : GlobalConfig.FileLocationsConfiguration.LetterheadBwHardcode;

        DocumentModel document = DocumentModel.Load(template);
        Block paragraph = document.Sections[0].Blocks[0];
        document.Sections[0].Blocks.Remove(paragraph);
        document.DefaultCharacterFormat.FontName = "Times New Roman";
        document.DefaultCharacterFormat.Size = 12;

        var letterDate = DateTime.Today.ToLongDateString();
        var nameLine = $"{model.FirstName} {model.LastName}";
        var prefix = model.Sex is null or "N" ? "Mr./Ms." : model.Sex == "M" ? "Mr." : "Ms.";

        Paragraph body = new(document);

        NewLine(body, document);
        NewLine(body, document);
        WriteLine(body, document, letterDate);
        NewLine(body, document);
        NewLine(body, document);
        WriteLine(body, document, nameLine);
        FormatAddressLines(body, document, model.Address1, model.Address2, model.City, model.State, model.Zip);
        NewLine(body, document);
        WriteLine(body, document,
        $"Dear {prefix} {model.LastName}:");
        NewLine(body, document);
        WriteLine(body, document,
            $"I would like to thank you for your interest in employment with Company Cooperative and the time you invested in applying for the {positionTitle.ToUpper()} position.");
        NewLine(body, document);
        WriteLine(body, document,
            $"Your application was reviewed, but unfortunately, you were not chosen for the position. Thank you again and we wish you every personal and professional success in the future.");
        NewLine(body, document);
        WriteLine(body, document,
            $"Sincerely,");
        NewLine(body, document);
        NewLine(body, document);
        NewLine(body, document);
        WriteLine(body, document,
            $"Tracy H. Resource");
        WriteLine(body, document,
            $"Director of Human Resources");
        WriteText(body, document,
            $"and Administration");

        document.Sections[0].Blocks.Add(body);

        document.Save(fileInfo.FullName);
        if (openWord)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region Helpers

    private static void WriteText(Paragraph body, DocumentModel document, string text) => body.Inlines.Add(new Run(document, text));

    private static void NewLine(Paragraph body, DocumentModel document) => body.Inlines.Add(new SpecialCharacter(document, SpecialCharacterType.LineBreak));

    private static void WriteLine(Paragraph body, DocumentModel document, string text)
    {
        WriteText(body, document, text);
        NewLine(body, document);
    }

    private static void FormatAddressLines(Paragraph body, DocumentModel document, string address1, string address2, string city, string state, string zip)
    {
        var skipped = 0;

        WriteText(body, document, address1.Trim().ToUpper());
        if (address2.Trim().Length > 0)
        {
            NewLine(body, document);
            WriteText(body, document, address2.Trim().ToUpper());
        }
        else
        {
            skipped++;
        }

        NewLine(body, document);
        WriteLine(body, document, $"{city}, {state} {zip.FormatZip()}");

        for (var i = 0; i < skipped; i++)
        {
            NewLine(body, document);
        }
    }

    #endregion
}