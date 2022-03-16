using System.Diagnostics;
using System.Text;

using HRSuite.Core.Models;

using Library.NET.Helpers;

using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HRSuite.Core.Reporting;
public class Worksheet : IWorksheet
{
    // Full on Portrait = 710 (713)
    // Full on Landscape = 935 (937)

    #region Constructor

    public Worksheet() => ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

    #endregion

    #region Employee Reports

    #region Age List

    public void CreateAgeListReport(List<EmployeeJobReportModel> models, bool openExcel, CancellationToken token)
    {
        var reportName = "Age List";
        List<string> pageHeaders = new() { "COMPANY COOPERATIVE", reportName, $"As of {DateTime.Today:MM/d/yyyy}" };
        List<string> pageFooters = new() { "Page &P of &N" };
        List<string> reportHeaders = new() { "Age", "Birthday", "Name", "Job Title", "Department" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            sheet.PrinterSettings.TopMargin = .75m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;
            sheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder header = new();
            StringBuilder footer = new();

            // Header
            for (var i = 0; i < pageHeaders.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                header.Append(SetHeaderFooter(pageHeaders[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeaders.Count - 1)
                {
                    header.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.RightAlignedText = header.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            // Add report headers
            for (var i = 0; i < reportHeaders.Count; i++)
            {
                using ExcelRange range = sheet.Cells[row, col++];
                if (i < 2)
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                }
                else
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.Indent = 1;
                }

                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 11;
                range.Style.Font.Bold = true;
                range.Value = reportHeaders[i];
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            row++;
            col = 1;
            var totalAge = 0;

            // Add report data
            foreach (EmployeeJobReportModel employee in models)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                using ExcelRange range = sheet.Cells[row, col, row, 5];
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;

                var age = GetCurrentAge(employee.Employee.Birthday);
                totalAge += age;
                var name = employee.Employee.FullName;
                DateTime birthday = employee.Employee.Birthday;
                var jobTitle = employee.JobTitle.Trim();
                var department = employee.FullDepartmentDescription.Trim();

                sheet.Row(row).Height = 18.75;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[row, col++].Value = age;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[row, col].Style.Numberformat.Format = "m/d/yyyy";
                sheet.Cells[row, col++].Value = birthday;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col++].Value = name;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col++].Value = jobTitle;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row++, col].Value = department;
                col = 1;
            }

            sheet.Cells[row, col, row, col + 4].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;

            using (ExcelRange range = sheet.Cells[row, col, row + 1, col])
            {
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
            }

            var totalEmployees = models.Count;
            var averageAge = decimal.Divide(totalAge, models.Count);

            sheet.Cells[row++, col].Value = $"Total Employees: {totalEmployees}";
            sheet.Cells[row, col].Value = $"Average Age: {averageAge:#.##}";

            //sheet.Cells.AutoFitColumns();

            var columnWidths = new int[] { 40, 75, 200, 160, 235 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            excel.SaveAs(fileInfo);
        }

        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region Birthday List

    public void CreateBirthdayListReport(List<EmployeeModel> models, bool openExcel, CancellationToken token)
    {
        var reportName = "Birthday List";
        List<string> pageHeaders = new() { "COMPANY COOPERATIVE", reportName, $"As of {DateTime.Today:MM/d/yyyy}" };
        List<string> pageFooters = new() { "Page &P of &N" };
        List<string> reportHeaders = new() { "Birthday", "Name" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            sheet.PrinterSettings.TopMargin = .75m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;
            sheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder header = new();
            StringBuilder footer = new();

            // Header
            for (var i = 0; i < pageHeaders.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                header.Append(SetHeaderFooter(pageHeaders[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeaders.Count - 1)
                {
                    header.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.RightAlignedText = header.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            var neededHeaderColumns = models.Count > 37 ? 2 : 1;
            var headerColumn = 1;
            // Add report headers
            while (headerColumn <= neededHeaderColumns)
            {
                for (var i = 0; i < reportHeaders.Count; i++)
                {
                    using ExcelRange range = sheet.Cells[row, col++];
                    if (i < 2)
                    {
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }
                    else
                    {
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }

                    range.Style.Font.Name = "Segoe UI";
                    range.Style.Font.Size = 11;
                    range.Style.Font.Bold = true;
                    range.Value = reportHeaders[i];
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                sheet.Cells[row, col++].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                headerColumn++;
            }

            row++;
            col = 1;

            models = models.OrderBy(_ => _.Birthday.Month).ThenBy(_ => _.Birthday.Day).ToList();

            // Add report data
            foreach (EmployeeModel employee in models)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                using ExcelRange range = sheet.Cells[row, col, row, col + 1];
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;

                var birthday = employee.Birthday.ToString("MM/dd");

                sheet.Row(row).Height = 18.75;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col++].Value = birthday;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row++, col].Value = employee.FullName;

                if (row == 38)
                {
                    row = 2;
                    col += 2;
                }
                else
                {
                    col--;
                }
            }

            var columnWidths = new int[] { 70, 210, 50, 70, 210, 100 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            excel.SaveAs(fileInfo);
        }

        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region CDL Report

    public void CreateCDLReport(List<EmployeeModel> models, bool openExcel, CancellationToken token)
    {
        var reportName = "CDL Report";
        List<string> pageHeaders = new() { "COMPANY COOPERATIVE", reportName, $"As of {DateTime.Today:MM/d/yyyy}" };
        List<string> pageFooters = new() { "Page &P of &N" };
        List<string> reportHeaders = new() { "Name", "DOB", "CDL License #", "CDL License Exp.", "CDL Medial Recert Exp.", "Driver License Restrictions" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.Orientation = eOrientation.Landscape;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            sheet.PrinterSettings.TopMargin = .75m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;
            sheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder header = new();
            StringBuilder footer = new();

            // Header
            for (var i = 0; i < pageHeaders.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                header.Append(SetHeaderFooter(pageHeaders[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeaders.Count - 1)
                {
                    header.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.RightAlignedText = header.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            // Add report headers
            for (var i = 0; i < reportHeaders.Count; i++)
            {
                using ExcelRange range = sheet.Cells[row, col++];

                if (i == 0)
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                }
                else if (i == 5)
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.Indent = 2;
                }
                else
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                }

                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 11;
                range.Style.Font.Bold = true;
                range.Value = reportHeaders[i];
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.WrapText = true;
            }

            row++;
            col = 1;

            // Add report data
            foreach (EmployeeModel employee in models)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                using ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count];
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;

                sheet.Row(row).Height = 18.75;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col++].Value = employee.FullName;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[row, col].Style.Numberformat.Format = "m/d/yyyy";
                sheet.Cells[row, col++].Value = employee.Birthday;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[row, col++].Value = employee.LicenseNumber.Trim();
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[row, col].Style.Numberformat.Format = "m/d/yyyy";
                sheet.Cells[row, col++].Value = employee.LicenseExp;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[row, col].Style.Numberformat.Format = "m/d/yyyy";
                sheet.Cells[row, col++].Value = employee.CDLMedExp;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Style.Indent = 2;
                sheet.Cells[row++, col].Value = employee.CDLDriverType.Trim();
                col = 1;
            }

            //sheet.Cells.AutoFitColumns();

            var columnWidths = new int[] { 200, 110, 110, 110, 110, 295 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            excel.SaveAs(fileInfo);
        }
        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region Employee Checklist

    public void CreateEmployeeChecklistReport(List<EmployeeModel> models, bool openExcel, CancellationToken token)
    {
        var reportName = "Employee Checklist";
        List<string> pageHeadersR = new() { "COMPANY COOPERATIVE", reportName };
        List<string> pageHeadersL = new() { $"Total Employees: {models.Count}" };
        List<string> pageFooters = new() { "Page &P of &N" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            sheet.PrinterSettings.TopMargin = .75m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder headerL = new();
            StringBuilder headerR = new();
            StringBuilder footer = new();

            // Headers
            for (var i = 0; i < pageHeadersL.Count; i++)
            {
                var fontSize = 12;
                var fontName = "Segoe UI";
                var isBold = true;
                var isItalic = true;

                headerL.Append(SetHeaderFooter(pageHeadersL[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeadersL.Count - 1)
                {
                    headerL.AppendLine();
                }
            }

            for (var i = 0; i < pageHeadersR.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                headerR.Append(SetHeaderFooter(pageHeadersR[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeadersR.Count - 1)
                {
                    headerR.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.LeftAlignedText = headerL.ToString();
            sheet.HeaderFooter.OddHeader.RightAlignedText = headerR.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            // Add report data
            foreach (EmployeeModel employee in models)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                using ExcelRange range = sheet.Cells[row, col];
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Row(row).Height = 18.75;
                sheet.Cells[row++, col].Value = $"___ {employee.FullName}";

                if (row == 38)
                {
                    row = 1;
                    col++;
                }
            }

            sheet.Cells.AutoFitColumns();

            excel.SaveAs(fileInfo);
        }
        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region Employee List

    public void CreateEmployeeListReport(List<EmployeeJobReportModel> models, bool openExcel, CancellationToken token)
    {
        var reportName = "Employee List";
        List<string> pageHeaders = new() { "COMPANY COOPERATIVE", reportName, $"As of {DateTime.Today:MM/d/yyyy}" };
        List<string> pageFooters = new() { "Page &P of &N" };
        List<string> reportHeaders = new() { "Name", "Emp. ID", "Job Title", "Department" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            sheet.PrinterSettings.TopMargin = .75m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;
            sheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder header = new();
            StringBuilder footer = new();

            // Header
            for (var i = 0; i < pageHeaders.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                header.Append(SetHeaderFooter(pageHeaders[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeaders.Count - 1)
                {
                    header.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.RightAlignedText = header.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            // Add report headers
            for (var i = 0; i < reportHeaders.Count; i++)
            {
                using ExcelRange range = sheet.Cells[row, col++];
                if (i == 0)
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                }
                else if (i == 1)
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                }
                else
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.Indent = 1;
                }

                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 11;
                range.Style.Font.Bold = true;
                range.Value = reportHeaders[i];
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            row++;
            col = 1;

            // Add report data
            foreach (EmployeeJobReportModel employee in models)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                using ExcelRange range = sheet.Cells[row, col, row, 5];
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;

                var name = employee.Employee.FullName;
                var employeeId = employee.Employee.Id;
                var jobTitle = employee.JobTitle.Trim();
                var department = employee.FullDepartmentDescription.Trim();

                sheet.Row(row).Height = 18.75;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col++].Value = name;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[row, col++].Value = employeeId;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col++].Value = jobTitle;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row++, col].Value = department;
                col = 1;
            }

            var totalEmployees = models.Count;
            sheet.Cells[row, col, row, reportHeaders.Count].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            using (ExcelRange range = sheet.Cells[row, 3])
            {
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
                range.Style.Font.Italic = true;
                range.Value = $"Total Employees: {totalEmployees}";
            }

            //sheet.Cells.AutoFitColumns();

            var columnWidths = new int[] { 200, 75, 200, 235 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            excel.SaveAs(fileInfo);
        }
        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region Promotions

    public void CreatePromotionsReport(List<PromotionReportModel> models, DateTime startDate, DateTime endDate, bool openExcel, CancellationToken token)
    {
        var reportName = "Promotions";
        List<string> pageHeaders = new() { "COMPANY COOPERATIVE", reportName, $"For Period {startDate.ToShortDateString()} - {endDate.ToShortDateString()}" };
        List<string> pageFooters = new() { "Page &P of &N" };
        List<string> reportHeaders = new() { "Name", "Job Title", "Department", "EEO", "Sex", "Ethnic", "Reason" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.Orientation = eOrientation.Landscape;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            sheet.PrinterSettings.TopMargin = 1m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;
            sheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder header = new();
            StringBuilder footer = new();

            // Header
            for (var i = 0; i < pageHeaders.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                header.Append(SetHeaderFooter(pageHeaders[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeaders.Count - 1)
                {
                    header.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.RightAlignedText = header.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            // Add report headers
            for (var i = 0; i < reportHeaders.Count; i++)
            {
                using ExcelRange range = sheet.Cells[row, col++];
                if (i == 0)
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                }
                else if (i is < 3 or 6)
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.Indent = 1;
                }
                else
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 11;
                range.Style.Font.Bold = true;
                range.Value = reportHeaders[i];
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            row++;
            col = 1;

            // Add report data
            foreach (PromotionReportModel employee in models)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                using ExcelRange range = sheet.Cells[row, col, row, 5];
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;

                var name = employee.FullName;
                var jobTitle = employee.JobTitle.Trim();
                var department = employee.FullDescription.Trim();
                var eeo = employee.EEOClass.Trim();
                var sex = employee.Sex.Trim();
                var ethnic = employee.Ethnicity.Trim();
                var reason = employee.Reason.Trim();

                sheet.Row(row).Height = 18.75;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col++].Value = name;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col++].Value = jobTitle;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col++].Value = department;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[row, col++].Value = eeo;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[row, col++].Value = sex;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[row, col++].Value = ethnic;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row++, col].Value = reason;
                col = 1;
            }

            sheet.Cells[row, col, row, reportHeaders.Count].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            using (ExcelRange range = sheet.Cells[row, col, row, col + 1])
            {
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
            }

            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[row, col++].Value = $"Total Promotions:";
            var promotionTotalAddress = sheet.Cells[2, 1, row - 1, 1].Address;
            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[row, col].Formula = $"COUNTA({promotionTotalAddress})";

            sheet.Calculate();
            //sheet.Cells.AutoFitColumns();

            var columnWidths = new int[] { 200, 210, 200, 60, 60, 60, 145 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            excel.SaveAs(fileInfo);
        }
        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region Requisition Profile

    public void CreateRequisitionProfileReport(RequisitionProfileReportModel requisition, bool openExcel, CancellationToken token)
    {
        var reportName = "Requisition Profile";
        List<string> pageHeadersR = new() { "COMPANY COOPERATIVE", reportName };
        List<string> pageFooters = new() { "Page &P of &N" };
        List<string> reportHeaders = new() { "ID", "Name", "Application Date", "Status" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            sheet.PrinterSettings.TopMargin = .75m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder headerR = new();
            StringBuilder footer = new();

            // Headers
            for (var i = 0; i < pageHeadersR.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                headerR.Append(SetHeaderFooter(pageHeadersR[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeadersR.Count - 1)
                {
                    headerR.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.RightAlignedText = headerR.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            // Add report data

            // Add Requisition data
            for (var r = 1; r < 10; r++)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                //if (r == 7) continue;
                row = r;
                using ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count];
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Row(row).Height = 18.75;

                var jobCode = requisition.Requisition.JobCode;
                var requisitionCode = requisition.Requisition.Code;
                var requisitionStatus = requisition.Requisition.Status;
                var eeoClass = requisition.JobCodeInfo.EEOClass;
                DateTime openDate = requisition.Requisition.OpenDate;
                var departmentFullDescription = requisition.CodeInfo.FirstOrDefault(_ => _.Type == "DEPT" && _.Code == requisition.JobCodeInfo.Department).FullDescription;
                var whyOpen = requisition.Requisition.ReasonOpen;
                var exempt = requisition.JobCodeInfo.Exempt;
                List<ApplicantRequisitionListModel> filledByList = requisition.Applicants.Where(_ => _.GeneralStatus == "OFFACC").ToList();
                StringBuilder filledBy = new();
                for (var i = 0; i < filledByList.Count; i++)
                {
                    ApplicantRequisitionListModel filledPerson = filledByList[i];
                    if (i == 0)
                    {
                        filledBy.Append($"{filledPerson.FirstName} {filledPerson.LastName}");
                    }
                    else
                    {
                        filledBy.Append($", {filledPerson.FirstName} {filledPerson.LastName}");
                    }
                }
                DateTime closedDate = requisition.Requisition.CloseDate;


                if (r == 1)
                {
                    range.Merge = true;
                    range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Font.Size = 14;
                    range.Style.Font.Bold = true;
                    sheet.Cells[row++, col].Value = jobCode;
                }
                else if (r == 2)
                {
                    sheet.Cells[row, col++].Value = "Code:";
                    sheet.Cells[row, col].Value = requisitionCode;
                }
                else if (r == 3)
                {
                    sheet.Cells[row, col++].Value = "Status:";
                    sheet.Cells[row, col].Value = requisitionStatus;
                }
                else if (r == 4)
                {
                    range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[row, col++].Value = "EEO Class:";
                    sheet.Cells[row, col++].Value = eeoClass;
                    sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    sheet.Cells[row, col].Style.Indent = 1;
                    sheet.Cells[row, col++].Value = "Date Open:";
                    sheet.Cells[row, col].Style.Numberformat.Format = "m/d/yyyy";
                    sheet.Cells[row, col++].Value = openDate;
                }
                else if (r == 5)
                {
                    sheet.Cells[row, col++].Value = "Department:";
                    sheet.Cells[row, col++].Value = departmentFullDescription;
                    sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    sheet.Cells[row, col].Style.Indent = 1;
                    sheet.Cells[row, col++].Value = "Date Closed:";
                    sheet.Cells[row, col].Style.Numberformat.Format = "m/d/yyyy";
                    sheet.Cells[row, col].Value = closedDate;

                }
                else if (r == 6)
                {
                    sheet.Cells[row, col++].Value = "Exempt:";
                    sheet.Cells[row, col++].Value = exempt;
                    sheet.Cells[row, col, row + 1, col].Merge = true;
                    sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    sheet.Cells[row, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[row, col].Style.Indent = 1;
                    sheet.Cells[row, col++].Value = "Why Open:";
                    sheet.Cells[row, col, row + 1, col].Merge = true;
                    sheet.Cells[row, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    sheet.Cells[row, col].Style.WrapText = true;
                    sheet.Cells[row, col].Value = whyOpen;
                }
                else if (r == 7)
                {
                    sheet.Cells[row, col++].Value = "Filled By:";
                    sheet.Cells[row, col].Value = filledBy.ToString();
                }
                else if (r == 8)
                {
                    range.Merge = true;
                    range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Font.Bold = true;
                    sheet.Cells[row, col].Value = "Assigned Applicants";
                }
                else if (r == 9)
                {
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Font.Bold = true;
                    for (var i = 0; i < reportHeaders.Count; i++)
                    {
                        if (i == 2)
                        {
                            sheet.Cells[row, col].Style.Indent = 1;
                        }
                        sheet.Cells[row, col++].Value = reportHeaders[i];
                    }
                }

                col = 1;
            }

            row++;

            foreach (ApplicantRequisitionListModel applicant in requisition.Applicants)
            {
                using ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count];
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Row(row).Height = 18.75;

                sheet.Cells[row, col++].Value = applicant.ApplicantId;
                sheet.Cells[row, col++].Value = applicant.FirstName + " " + applicant.LastName;
                sheet.Cells[row, col].Style.Numberformat.Format = "m/d/yyyy";
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col++].Value = applicant.ApplicationDate;
                sheet.Cells[row++, col].Value = applicant.GeneralStatus;

                col = 1;
            }

            var totalApplicants = requisition.Applicants.Count;

            using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
            {
                range.Merge = true;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
                sheet.Row(row).Height = 18.75;
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.Cells[row, col].Value = $"Total Applicants: {totalApplicants}";
            }

            row += 2;

            using (ExcelRange range = sheet.Cells[row, col])
            {
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
                sheet.Row(row).Height = 18.75;
                sheet.Cells[row, col++].Value = $"Notes:";
            }

            using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
            {
                range.Merge = true;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
                sheet.Row(row).Height = 18.75;
                range.Style.WrapText = true;
                sheet.Cells[row, col].Value = requisition.Requisition.Comment ?? string.Empty;
            }

            var columnWidths = new int[] { 85, 155, 155, 315 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            excel.SaveAs(fileInfo);
        }
        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region Safety Meeting Atten. List

    public void CreateSafetyMeetingAttenListReport(List<EmployeeJobReportModel> models, bool openExcel, CancellationToken token)
    {
        var reportName = "Safety Training Attendance Record";
        List<string> pageHeaders = new() { "COMPANY COOPERATIVE", reportName };
        List<string> pageFooters = new() { "Page &P of &N" };
        List<string> reportHeaders = new() { "Name", "Signature", "Job Title" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            sheet.PrinterSettings.TopMargin = 1m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;
            sheet.PrinterSettings.RepeatRows = new ExcelAddress("$12:$12");

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder header = new();
            StringBuilder footer = new();

            // Header
            for (var i = 0; i < pageHeaders.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                header.Append(SetHeaderFooter(pageHeaders[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeaders.Count - 1)
                {
                    header.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.RightAlignedText = header.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            // Add report headers
            for (var i = 1; i < 12; i++)
            {
                row = i;
                using ExcelRange range = sheet.Cells[row, col, row, 7];
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 12;
                range.Style.Font.Bold = true;
                sheet.Row(row).Height = 18.75;

                if (i == 1)
                {
                    sheet.Cells[row, col++].Value = "Instructor";
                    sheet.Cells[row, col++, row, col++].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    sheet.Cells[row, col++].Value = "Date";
                    sheet.Cells[row, col++].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    sheet.Cells[row, col++].Value = "Time";
                    sheet.Cells[row, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                else if (i == 3)
                {
                    range.Merge = true;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Value = "Topics";
                }
                else if (i > 3)
                {
                    sheet.Cells[row, col++].Value =
                        i == 4 ? "Topic #1" :
                        i == 5 ? "Topic #2" :
                        i == 6 ? "Topic #3" :
                        i == 7 ? "Comments" :
                        "";

                    sheet.Cells[row, col, row, 7].Merge = true;
                    sheet.Cells[row, col, row, 7].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                col = 1;
            }

            row++;
            sheet.Row(row).Height = 18.75;
            using (ExcelRange range = sheet.Cells[row, col, row, col + 1])
            {
                range.Merge = true;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 12;
                range.Style.Font.Bold = true;
                sheet.Cells[row, col].Value = "Name";
            }

            col = 3;
            using (ExcelRange range = sheet.Cells[row, col, row, col + 2])
            {
                range.Merge = true;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 12;
                range.Style.Font.Bold = true;
                range.Style.Indent = 1;
                sheet.Cells[row, col].Value = "Signature";
            }

            col = 6;
            using (ExcelRange range = sheet.Cells[row, col, row, col + 1])
            {
                range.Merge = true;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 12;
                range.Style.Font.Bold = true;
                range.Style.Indent = 1;
                sheet.Cells[row++, col].Value = "Job Title";
            }

            col = 1;

            // Add report data
            foreach (EmployeeJobReportModel employee in models)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                sheet.Row(row).Height = 18.75;
                using (ExcelRange range = sheet.Cells[row, col, row, col + 1])
                {
                    range.Merge = true;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.Font.Name = "Segoe UI";
                    range.Style.Font.Size = 10;
                    sheet.Cells[row, col].Value = employee.Employee.FullName;
                }

                col = 3;
                using (ExcelRange range = sheet.Cells[row, col, row, col + 2])
                {
                    range.Merge = true;
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Font.Name = "Segoe UI";
                    range.Style.Font.Size = 10;
                }

                col = 6;
                using (ExcelRange range = sheet.Cells[row, col, row, col + 1])
                {
                    range.Merge = true;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.Font.Name = "Segoe UI";
                    range.Style.Font.Size = 10;
                    range.Style.Indent = 1;
                    sheet.Cells[row++, col].Value = employee.JobTitle;
                }
                col = 1;
            }

            var totalEmployees = models.Count;
            using (ExcelRange range = sheet.Cells[row, col, row, 7])
            {
                sheet.Row(row).Height = 18.75;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
                range.Style.Font.Italic = true;
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
                range.Merge = true;
                range.Value = $"Total Employees: {totalEmployees}";
            }

            //sheet.Cells.AutoFitColumns();

            var columnWidths = new int[] { 90, 110, 50, 50, 210, 50, 150 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            excel.SaveAs(fileInfo);
        }
        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region Service List

    public void CreateServiceListReport(List<EmployeeJobReportModel> models, bool openExcel, CancellationToken token)
    {
        var reportName = "Service List";
        List<string> pageHeaders = new() { "COMPANY COOPERATIVE", reportName, $"As of {DateTime.Today:MM/d/yyyy}" };
        List<string> pageFooters = new() { "Page &P of &N" };
        List<string> reportHeaders = new() { "Seniority Yrs, Mo", "Last Hire Date", "Name", "Job Title", "Department" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            sheet.PrinterSettings.TopMargin = .75m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;
            sheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder header = new();
            StringBuilder footer = new();

            // Header
            for (var i = 0; i < pageHeaders.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                header.Append(SetHeaderFooter(pageHeaders[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeaders.Count - 1)
                {
                    header.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.RightAlignedText = header.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            // Add report headers
            for (var i = 0; i < reportHeaders.Count; i++)
            {
                using ExcelRange range = sheet.Cells[row, col++];
                if (i == 0)
                {
                    range.Style.WrapText = true;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                }
                else if (i == 1)
                {
                    range.Style.WrapText = true;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                }
                else
                {
                    range.Style.Indent = 1;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                }

                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 11;
                range.Style.Font.Bold = true;
                sheet.Row(row).Height = 33;
                range.Value = reportHeaders[i];
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            row++;
            col = 1;

            List<int> seniorityMonthAmountsList = new();

            // Add report data
            foreach (EmployeeJobReportModel employee in models)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                using ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count];
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                DateTime seniorityDate = employee.Employee.SeniorityDate;

                var totalMonths = GetServiceLength(seniorityDate);
                var y = decimal.Divide(totalMonths, 12);
                var m = (y - (int)y) * 12;
                var seniorityLength = $"{(int)y}y, {(int)m}m";

                var name = employee.Employee.FullName;
                var jobTitle = employee.JobTitle.Trim();
                var department = employee.FullDepartmentDescription.Trim();

                seniorityMonthAmountsList.Add(totalMonths);

                sheet.Row(row).Height = 18.75;
                sheet.Cells[row, col++].Value = seniorityLength;
                sheet.Cells[row, col].Style.Numberformat.Format = "m/d/yyyy";
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[row, col++].Value = seniorityDate;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col++].Value = name;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col++].Value = jobTitle;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row++, col].Value = department;
                col = 1;
            }

            sheet.Cells[row, col, row, col + 4].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;

            using (ExcelRange range = sheet.Cells[row, col, row + 1, col])
            {
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
            }

            var totalEmployees = models.Count;
            var totalSeniorityMonths = seniorityMonthAmountsList.Sum();
            var averageSeniorityMonths = decimal.Divide(totalSeniorityMonths, models.Count);

            var averageYears = averageSeniorityMonths / 12m;
            var averageMonths = (averageYears - (int)averageYears) * 12;

            sheet.Cells[row++, col].Value = $"Total Employees: {totalEmployees}";
            sheet.Cells[row, col].Value = $"Average Service: {(int)averageYears}y, {(int)averageMonths}m";

            //sheet.Cells.AutoFitColumns();

            var columnWidths = new int[] { 80, 75, 200, 180, 175 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            excel.SaveAs(fileInfo);
        }
        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #endregion

    #region EEO Reports

    #region EEO New Hire & Termination Headcount Detail

    public void CreateEEOHeadcountDetailReport(List<EEOHeadcountDetailReportModel> model, DateTime startDate, DateTime endDate, bool newHireReport, bool openExcel, CancellationToken token)
    {
        var reportType = newHireReport ? "New Hire" : "Termination";
        var reportName = $"EEO {reportType} Headcount Detail";
        List<string> pageHeaders = new() { "COMPANY COOPERATIVE", reportName, $"From {startDate.ToShortDateString()} to {endDate.ToShortDateString()}" };
        List<string> pageFooters = new() { "Page &P of &N" };
        List<string> reportHeaders = new() { "Name", "Job Title - Department", "Effective Date", "Disabled", "Disabled Veteran", "Protected Veteran" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.Orientation = eOrientation.Landscape;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            //sheet.PrinterSettings.TopMargin = .75m;
            sheet.PrinterSettings.TopMargin = 1m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;
            sheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder header = new();
            StringBuilder footer = new();

            // Header
            for (var i = 0; i < pageHeaders.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                header.Append(SetHeaderFooter(pageHeaders[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeaders.Count - 1)
                {
                    header.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.RightAlignedText = header.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            // Add report headers
            for (var i = 0; i < reportHeaders.Count; i++)
            {
                using ExcelRange range = sheet.Cells[row, col++];

                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                range.Style.Indent = 1;
                range.Style.WrapText = true;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 11;
                range.Style.Font.Bold = true;
                range.Value = reportHeaders[i];
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
            }

            row++;
            col = 1;

            // Add report data

            IEnumerable<IGrouping<string, EEOHeadcountDetailReportModel>> eeoClassGroups = model.GroupBy(_ => _.EEOClass);

            foreach (IGrouping<string, EEOHeadcountDetailReportModel> eeoClassGroup in eeoClassGroups)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                var eeoClass = eeoClassGroup.Key;

                using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
                {
                    range.Merge = true;
                    range.Style.Indent = 1;
                    range.Style.Font.Name = "Segoe UI";
                    range.Style.Font.Size = 11;
                    range.Style.Font.Bold = true;
                    sheet.Row(row).Height = 18.75;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Value = $"EEO Class: {eeoClass}";
                }
                row++;

                IEnumerable<IGrouping<string, EEOHeadcountDetailReportModel>> genderGroups = eeoClassGroup.GroupBy(_ => _.Sex);

                foreach (IGrouping<string, EEOHeadcountDetailReportModel> genderGroup in genderGroups)
                {
                    var gender = genderGroup.Key;

                    using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
                    {
                        range.Merge = true;
                        range.Style.Indent = 1;
                        range.Style.Font.Name = "Segoe UI";
                        range.Style.Font.Size = 10;
                        sheet.Row(row).Height = 18.75;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Value = $"Gender is {gender}";
                    }
                    row++;

                    IEnumerable<IGrouping<string, EEOHeadcountDetailReportModel>> ethnicGroups = genderGroup.GroupBy(_ => _.Ethnicity);

                    foreach (IGrouping<string, EEOHeadcountDetailReportModel> ethnicGroup in ethnicGroups)
                    {
                        var ethnic = ethnicGroup.Key;

                        using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
                        {
                            range.Merge = true;
                            range.Style.Indent = 1;
                            range.Style.Font.Name = "Segoe UI";
                            range.Style.Font.Size = 10;
                            sheet.Row(row).Height = 18.75;
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            range.Value = $"Ethnicity is {ethnic}";
                        }
                        row++;

                        foreach (EEOHeadcountDetailReportModel employee in ethnicGroup)
                        {
                            var name = employee.FullName;
                            var jobTitleDepartment = employee.JobTitleDepartment;
                            DateTime effectiveDate = employee.EffectiveDate;
                            var disabled = employee.Disabled;
                            var disVet = employee.DisabledVet;
                            var proVet = employee.ProtectedVet;

                            using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
                            {
                                range.Style.Font.Name = "Segoe UI";
                                range.Style.Font.Size = 10;
                                sheet.Row(row).Height = 18.75;
                            }
                            sheet.Cells[row, col].Style.Indent = 1;
                            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            sheet.Cells[row, col++].Value = name;
                            sheet.Cells[row, col].Style.Indent = 1;
                            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            sheet.Cells[row, col++].Value = jobTitleDepartment;
                            sheet.Cells[row, col].Style.Indent = 1;
                            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            sheet.Cells[row, col].Style.Numberformat.Format = "m/d/yyyy";
                            sheet.Cells[row, col++].Value = effectiveDate;
                            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            sheet.Cells[row, col++].Value = disabled;
                            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            sheet.Cells[row, col++].Value = disVet;
                            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            sheet.Cells[row, col].Value = proVet;

                            row++;
                            col = 1;
                        }
                        var ethnicTotal = ethnicGroup.Count();

                        using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
                        {
                            range.Style.Font.Name = "Segoe UI";
                            range.Style.Font.Size = 10;
                            range.Style.Font.Bold = true;
                            sheet.Row(row).Height = 18.75;
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                        sheet.Cells[row, col].Style.Indent = 1;
                        sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Cells[row, col++].Value = $"Total Ethnicity - {ethnic}";
                        sheet.Cells[row, col].Style.Indent = 1;
                        sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Cells[row, col].Value = ethnicTotal;

                        row++;
                        col = 1;
                    }

                    var genderTotal = genderGroup.Count();

                    using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
                    {
                        range.Style.Font.Name = "Segoe UI";
                        range.Style.Font.Size = 10;
                        range.Style.Font.Bold = true;
                        sheet.Row(row).Height = 18.75;
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    sheet.Cells[row, col].Style.Indent = 1;
                    sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    sheet.Cells[row, col++].Value = $"Total Gender - {gender}";
                    sheet.Cells[row, col].Style.Indent = 1;
                    sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    sheet.Cells[row, col].Value = genderTotal;

                    row++;
                    col = 1;
                }

                var eeoClassTotal = eeoClassGroup.Count();

                using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
                {
                    range.Style.Font.Name = "Segoe UI";
                    range.Style.Font.Size = 10;
                    range.Style.Font.Bold = true;
                    sheet.Row(row).Height = 18.75;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col++].Value = $"Total EEO Class - {eeoClass}";
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Value = eeoClassTotal;

                row++;
                col = 1;
            }

            var totalEmployees = model.Count;

            using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
            {
                range.Merge = true;
                range.Style.Indent = 1;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
                sheet.Row(row).Height = 18.75;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Value = $"Total Employees Listed:     {totalEmployees}";
            }

            var columnWidths = new int[] { 200, 350, 120, 80, 80, 105 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            excel.SaveAs(fileInfo);
        }
        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region EEO New Hire & Termination Summary

    public void CreateEEOHeadcountSummaryReport(List<EEOHeadcountDetailReportModel> model, DateTime startDate, DateTime endDate, bool newHireReport, bool openExcel, CancellationToken token)
    {
        var reportType = newHireReport ? "New Hire" : "Termination";
        var reportName = $"EEO {reportType} Summary";
        List<string> pageHeaders = new() { "COMPANY COOPERATIVE", reportName, $"From {startDate.ToShortDateString()} to {endDate.ToShortDateString()}" };
        List<string> pageFooters = new() { "Page &P of &N" };
        List<string> reportHeaders = new() { "EEO Class", "Total", "M", "F", "W", "B", "A", "I", "N", "T", "W", "B", "A", "I", "N", "T" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.Orientation = eOrientation.Landscape;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            //sheet.PrinterSettings.TopMargin = .75m;
            sheet.PrinterSettings.TopMargin = 1m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;
            sheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$2");

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder header = new();
            StringBuilder footer = new();

            // Header
            for (var i = 0; i < pageHeaders.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                header.Append(SetHeaderFooter(pageHeaders[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeaders.Count - 1)
                {
                    header.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.RightAlignedText = header.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            // Add report headers' titles
            sheet.Row(row).Height = 30;
            using (ExcelRange range = sheet.Cells[row, 3, row, 4])
            {
                range.Merge = true;
                range.Style.WrapText = true;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Value = "Hispanic or Latino";
            }

            using (ExcelRange range = sheet.Cells[row, 5, row, 10])
            {
                range.Merge = true;
                range.Style.WrapText = true;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Value = "MALE (Not Hispanic OR Latino)";
            }

            using (ExcelRange range = sheet.Cells[row, 11, row, 16])
            {
                range.Merge = true;
                range.Style.WrapText = true;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Value = "FEMALE (Not Hispanic OR Latino)";
            }

            row++;

            // Add report headers
            for (var i = 0; i < reportHeaders.Count; i++)
            {
                using ExcelRange range = sheet.Cells[row, col++];

                if (i == 0)
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.Indent = 1;
                }
                else
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                if (i is 1 or 3 or 9 or 15)
                {
                    range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                sheet.Row(row).Height = 18.75;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 11;
                range.Style.Font.Bold = true;
                range.Value = reportHeaders[i];
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
            }

            row++;
            col = 1;

            // Add report data

            var totalEmployees = model.Count;
            IEnumerable<IGrouping<string, EEOHeadcountDetailReportModel>> eeoClassGroups = model.GroupBy(_ => _.EEOClass);

            foreach (IGrouping<string, EEOHeadcountDetailReportModel> eeoClassGroup in eeoClassGroups)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                var eeoClassDescription = eeoClassGroup.Select(_ => _.EEOClassDescription).FirstOrDefault();

                var hispanicLatinoMaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Hispanic/Latino" && _.Sex == "Male");
                var hispanicLatinoFemaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Hispanic/Latino" && _.Sex == "Female");
                var whiteMaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "White" && _.Sex == "Male");
                var whiteFemaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "White" && _.Sex == "Female");
                var blackMaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Black/African American" && _.Sex == "Male");
                var blackFemaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Black/African American" && _.Sex == "Female");
                var asianMaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Asian" && _.Sex == "Male");
                var asianFemaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Asian" && _.Sex == "Female");
                var nativeMaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Amer Indian/Alaska Native" && _.Sex == "Male");
                var nativeFemaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Amer Indian/Alaska Native" && _.Sex == "Female");
                var hawaiiMaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Native Hawaii/Pacific Isln" && _.Sex == "Male");
                var hawaiiFemaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Native Hawaii/Pacific Isln" && _.Sex == "Female");
                var twoOrMoreMaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Two or More Races" && _.Sex == "Male");
                var twoOrMoreFemaleCount = eeoClassGroup.Count(_ => _.Ethnicity == "Two or More Races" && _.Sex == "Female");

                using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
                {
                    range.Style.Font.Name = "Segoe UI";
                    range.Style.Font.Size = 10;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                sheet.Row(row).Height = 18.75;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col++].Value = eeoClassDescription;
                sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.Cells[row, col++].Formula = $"SUM(C{row}:P{row})";
                sheet.Cells[row, col++].Value = hispanicLatinoMaleCount;
                sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.Cells[row, col++].Value = hispanicLatinoFemaleCount;
                sheet.Cells[row, col++].Value = whiteMaleCount;
                sheet.Cells[row, col++].Value = blackMaleCount;
                sheet.Cells[row, col++].Value = asianMaleCount;
                sheet.Cells[row, col++].Value = nativeMaleCount;
                sheet.Cells[row, col++].Value = hawaiiMaleCount;
                sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.Cells[row, col++].Value = twoOrMoreMaleCount;
                sheet.Cells[row, col++].Value = whiteFemaleCount;
                sheet.Cells[row, col++].Value = blackFemaleCount;
                sheet.Cells[row, col++].Value = asianFemaleCount;
                sheet.Cells[row, col++].Value = nativeFemaleCount;
                sheet.Cells[row, col++].Value = hawaiiFemaleCount;
                sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.Cells[row++, col].Value = twoOrMoreFemaleCount;
                col = 1;
            }

            using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
            {
                sheet.Row(row).Height = 18.75;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            // Totals
            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[row, col].Style.Indent = 1;
            sheet.Cells[row, col++].Value = "Total";
            sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col++].Formula = $"SUM(B3:B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(C3:C{row - 1})";
            sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col++].Formula = $"SUM(D3:D{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(E3:E{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(F3:F{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(G3:G{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(H3:H{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(I3:I{row - 1})";
            sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col++].Formula = $"SUM(J3:J{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(K3:K{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(L3:L{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(M3:M{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(N3:N{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(O3:O{row - 1})";
            sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col].Formula = $"SUM(P3:P{row - 1})";

            row++;
            col = 1;

            // Percentages
            using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
            {
                sheet.Row(row).Height = 18.75;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                range.Style.Numberformat.Format = "0%";
            }

            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            sheet.Cells[row, col].Style.Indent = 1;
            sheet.Cells[row, col++].Value = "Percentages";
            sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col++].Formula = $"SUM(B{row - 1}/B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(C{row - 1}/B{row - 1})";
            sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col++].Formula = $"SUM(D{row - 1}/B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(E{row - 1}/B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(F{row - 1}/B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(G{row - 1}/B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(H{row - 1}/B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(I{row - 1}/B{row - 1})";
            sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col++].Formula = $"SUM(J{row - 1}/B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(K{row - 1}/B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(L{row - 1}/B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(M{row - 1}/B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(N{row - 1}/B{row - 1})";
            sheet.Cells[row, col++].Formula = $"SUM(O{row - 1}/B{row - 1})";
            sheet.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col].Formula = $"SUM(P{row - 1}/B{row - 1})";

            row++;
            col = 1;

            // Percent Male
            sheet.Row(row).Height = 18.75;
            sheet.Cells[row, col].Style.Indent = 1;
            sheet.Cells[row, col].Style.Font.Name = "Segoe UI";
            sheet.Cells[row, col].Style.Font.Size = 10;
            sheet.Cells[row, col++].Value = "Percent Male";
            sheet.Cells[row, col].Style.Numberformat.Format = "0%";
            sheet.Cells[row, col].Style.Font.Name = "Segoe UI";
            sheet.Cells[row, col].Style.Font.Size = 10;
            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[row, col].Formula = $"SUM((SUM(E{row - 2}:J{row - 2})+C{row - 2})/B{row - 2})";

            row++;
            col = 1;

            // Percent Female
            sheet.Row(row).Height = 18.75;
            sheet.Cells[row, col].Style.Indent = 1;
            sheet.Cells[row, col].Style.Font.Name = "Segoe UI";
            sheet.Cells[row, col].Style.Font.Size = 10;
            sheet.Cells[row, col++].Value = "Percent Female";
            sheet.Cells[row, col].Style.Numberformat.Format = "0%";
            sheet.Cells[row, col].Style.Font.Name = "Segoe UI";
            sheet.Cells[row, col].Style.Font.Size = 10;
            sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[row, col].Formula = $"SUM((SUM(K{row - 3}:P{row - 3})+C{row - 3})/B{row - 3})";

            var columnWidths = new int[] { 315, 60, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            sheet.Calculate();

            excel.SaveAs(fileInfo);
        }
        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region EEO Requisition Profile

    public void CreateEEORequisitionProfileReport(RequisitionProfileReportModel model, bool openExcel, CancellationToken token)
    {
        var reportName = "EEO Requisition Profile";
        List<string> pageHeaders = new() { "COMPANY COOPERATIVE", reportName, $"Requisition Code: {model.Requisition.Code}" };
        List<string> pageFooters = new() { "Page &P of &N" };
        List<string> reportHeaders = new() { "Application Date", "Applicant Name", "Sex", "Ethnic", "Protected Veteran", "Disbled Veteran", "Interviewed", "Disposition Code with Two Lines" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.Orientation = eOrientation.Landscape;
            sheet.PrinterSettings.HeaderMargin = .25m;
            sheet.PrinterSettings.FooterMargin = .25m;
            sheet.PrinterSettings.TopMargin = .75m;
            sheet.PrinterSettings.BottomMargin = .5m;
            sheet.PrinterSettings.LeftMargin = .25m;
            sheet.PrinterSettings.RightMargin = .25m;
            sheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");

            var row = 1;
            var col = 1;

            // Add page headers and footers
            StringBuilder header = new();
            StringBuilder footer = new();

            // Header
            for (var i = 0; i < pageHeaders.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = false;

                if (i == 0)
                {
                    fontSize = 14;
                    isBold = true;
                }
                else if (i == 1)
                {
                    fontSize = 12;
                }
                else
                {
                    isItalic = true;
                }

                header.Append(SetHeaderFooter(pageHeaders[i], fontSize, fontName, isBold, isItalic));

                if (i < pageHeaders.Count - 1)
                {
                    header.AppendLine();
                }
            }

            // Footer
            for (var i = 0; i < pageFooters.Count; i++)
            {
                var fontSize = 10;
                var fontName = "Segoe UI";
                var isBold = false;
                var isItalic = true;

                footer.Append(SetHeaderFooter(pageFooters[i], fontSize, fontName, isBold, isItalic));

                if (i < pageFooters.Count - 1)
                {
                    footer.AppendLine();
                }
            }

            sheet.HeaderFooter.OddHeader.RightAlignedText = header.ToString();
            sheet.HeaderFooter.OddFooter.RightAlignedText = footer.ToString();
            sheet.HeaderFooter.differentFirst = false;
            sheet.HeaderFooter.ScaleWithDocument = false;

            // Add report headers
            for (var i = 0; i < reportHeaders.Count; i++)
            {
                using ExcelRange range = sheet.Cells[row, col++];

                if (i == 0)
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                }
                else if (i is 1 or 7)
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    range.Style.Indent = 1;
                }
                else
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 11;
                range.Style.Font.Bold = true;
                range.Value = reportHeaders[i];
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.WrapText = true;
            }

            row++;
            col = 1;

            // Add report data
            using (ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count])
            {
                range.Merge = true;
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;
                range.Style.Font.Bold = true;
                sheet.Row(row).Height = 18.75;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                range.Value = $"Job Requisition: {model.Requisition.Description}   Job Title: {model.JobCodeInfo.JobTitle}   EEO Class: {model.JobCodeInfo.EEOClass}";
            }

            row++;

            foreach (ApplicantRequisitionListModel applicant in model.Applicants)
            {
                // Check if user wants to cancel the report generation
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                using ExcelRange range = sheet.Cells[row, col, row, reportHeaders.Count];
                range.Style.Font.Name = "Segoe UI";
                range.Style.Font.Size = 10;

                sheet.Row(row).Height = 18.75;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[row, col].Style.Numberformat.Format = "m/d/yyyy";
                sheet.Cells[row, col++].Value = applicant.ApplicationDate;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col++].Value = applicant.FullName;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[row, col++].Value = applicant.Sex;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[row, col++].Value = applicant.Ethnicity;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[row, col++].Value = applicant.ProtectedVet;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[row, col++].Value = applicant.DisabledVet;
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[row, col++].Value = applicant.Interviewed.ToLower() == "true" ? "Y" : "N";
                sheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[row, col].Style.Indent = 1;
                sheet.Cells[row, col].Value = applicant.GeneralStatus;
                row++;
                col = 1;
            }

            var columnWidths = new int[] { 90, 350, 40, 50, 85, 85, 100, 135 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            excel.SaveAs(fileInfo);
        }
        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #endregion

    #region Job Postings

    public void CreateVacancyPosting(RequisitionModel model, JobCodeModel jobCode, DateTime deadlineDate, string hiringSupervisor, bool openExcel, CancellationToken token)
    {
        var reportName = $"Job Vacancy Posting - {model.Code}";
        var isInternal = model.Internal;
        List<string> pageHeaders = new() { "COMPANY COOPERATIVE", $"{(isInternal ? "INTERNAL" : "EXTERNAL")} HIRE", "JOB VACANCY POSTING" };

        FileInfo fileInfo = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{reportName}.xlsx"));

        if (File.Exists(fileInfo.FullName))
        {
            File.Delete(fileInfo.FullName);
        }

        using (ExcelPackage excel = new())
        {
            // Add Worksheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(reportName);

            // Set Layout settings
            sheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            sheet.PrinterSettings.HeaderMargin = .5m;
            sheet.PrinterSettings.FooterMargin = .5m;
            sheet.PrinterSettings.TopMargin = 1.5m;
            sheet.PrinterSettings.BottomMargin = 1.0m;
            sheet.PrinterSettings.LeftMargin = 1.0m;
            sheet.PrinterSettings.RightMargin = .75m;

            var row = 1;
            var col = 1;

            // Add report headers
            for (var i = 0; i < pageHeaders.Count; i++)
            {
                using ExcelRange range = sheet.Cells[row, col++, row++, col--];
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                range.Merge = true;

                range.Style.Font.Name = "Calibri";
                range.Style.Font.Size = 20;
                range.Style.Font.Bold = true;
                range.Value = pageHeaders[i];
            }

            sheet.Row(row++).Height = 40.PixelsToInchesHeight();

            col = 1;

            using (ExcelRange range = sheet.Cells[row, col, 22, 2])
            {
                range.Style.Font.Name = "Calibri";
                range.Style.Font.Size = 12;
                range.Style.Font.Bold = true;
            }

            sheet.Cells[row, col++].Value = "Date of Posting:";
            sheet.Cells[row, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col--].Value = $"{model.OpenDate.ToLongDateString()}, at 8:00am";
            row += 2;
            sheet.Cells[row, col++].Value = "Posting Deadline:";
            sheet.Cells[row, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col--].Value = $"{deadlineDate.ToLongDateString()}, at 5:00pm";
            row += 2;
            sheet.Cells[row, col++].Value = "Expected Hire Date:";
            sheet.Cells[row, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col--].Value = $"Immediately";
            row += 2;

            using (ExcelRange range = sheet.Cells[row, col++, row, col--])
            {
                range.IsRichText = true;
                range.Style.WrapText = true;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                range.Merge = true;
                range.Style.Font.Name = "Calibri";
                range.Style.Font.Size = 12;
                range.Style.Font.Bold = true;

                if (isInternal)
                {
                    ExcelRichText para1 = range.RichText.Add("The Cooperative invites qualified interested individuals to apply for the following position. Applicants must complete an Internal Employement Application form that will be provided by the Cooperative.");
                    ExcelRichText para2 = range.RichText.Add("\n\nPer Policy 2013, employees transferred, reassigned, or promoted from one job classification to another job classification shall not be eligible to bid on any job previously held within the last six months. Additionally, new employees are prohibited from applying for a different position within the Cooperative for a period of one year from their date of hire.");
                    ExcelRichText para3 = range.RichText.Add("\n\nAny questions concerning the position may be directed to the Director of Human Resources & Administration, Tracy H. Resource, 123-555-3107, Ext. 228 or 1-800-555-2217, Ext. 228.");
                }
                else
                {
                    ExcelRichText para1Intro = range.RichText.Add("The Cooperative invites qualified interested individuals to apply for the following position. Applicants may find the application during the time period specified on our website, ");
                    ExcelRichText website = range.RichText.Add("www.company.com");
                    ExcelRichText para1Middle = range.RichText.Add(", or at our offices at 1300 Hwy 24, Newport and 450 McCotter Blvd., in Havelock. The completion of an application form specific to the position is required. Applications should be submitted to ");
                    ExcelRichText email = range.RichText.Add("jobs@company.com");
                    ExcelRichText para1End = range.RichText.Add(" or in our offices.");
                    ExcelRichText para2 = range.RichText.Add("\n\nAny questions concerning the position may be directed to the Director of Human Resources & Administration, Tracy H. Resource, 123-555-3107, Ext. 228 or 1-800-555-2217, Ext. 228.");

                    email.UnderLine = true;
                    website.UnderLine = true;
                }
            }

            sheet.Row(row++).Height = 275.PixelsToInchesHeight();

            sheet.Cells[row, col++].Value = "Position:";
            sheet.Cells[row, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col--].Value = $"{jobCode.JobTitle}";
            row += 2;
            sheet.Cells[row, col++].Value = "Department";
            sheet.Cells[row, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col--].Value = $"{jobCode.Department}";
            row += 2;
            sheet.Cells[row, col++].Value = "Hiring Supervisor";
            sheet.Cells[row, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            sheet.Cells[row, col--].Value = $"{hiringSupervisor}";

            // EEO Message
            using (ExcelRange range = sheet.Cells[22, 1, 22, 2])
            {
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                range.Merge = true;
                range.Style.Font.Bold = true;
                range.Style.Font.Italic = true;
                range.Value = "An Equal Opportunity Affirmative Action Employer/M/F/D/V";
            }

            var columnWidths = new int[] { 175, 425 };

            for (var i = 1; i <= columnWidths.Length; i++)
            {
                sheet.Column(i).Width = columnWidths[i - 1].PixelsToInchesWidth();
            }

            excel.SaveAs(fileInfo);
        }

        if (openExcel)
        {
            ProcessStartInfo processStartInfo = new() { FileName = $"\"{fileInfo.FullName}\"", UseShellExecute = true };
            _ = Process.Start(processStartInfo);
        }
    }

    #endregion

    #region Private Methods

    private static int GetCurrentAge(DateTime birthdate)
    {
        var years = DateTime.Today.Year - birthdate.Year;
        if (DateTime.Today.DayOfYear < birthdate.DayOfYear)
        {
            years--;
        }
        return years;
    }

    private static int GetServiceLength(DateTime serviceDate)
    {
        var years = GetCurrentAge(serviceDate);
        var months = DateTime.Today.Month - serviceDate.Month;
        if (months == 0)
        {
            var days = DateTime.Today.Day - serviceDate.Day;
            if (days < 0)
            {
                months = 11;
            }
        }
        else if (months < 0)
        {
            months += 12;
        }

        var seniorityMo = $"{years}y, {months}m";

        var totalMonths = years * 12 + months;

        var y = decimal.Divide(totalMonths, 12);
        var m = (y - (int)y) * 12;

        var seniorityMo2 = $"{(int)y}y, {(int)m}m";

        return totalMonths;
    }

    private static string SetHeaderFooter(string text, int fontSize, string fontName = "Segoe UI", bool isBold = false, bool isItalic = false)
    {
        string style;

        if (isBold && isItalic)
        {
            style = ",Bold Italic";
        }
        else if (isBold)
        {
            style = ",Bold";
        }
        else if (isItalic)
        {
            style = ",Italic";
        }
        else
        {
            style = ",Regular";
        }

        return $"&{fontSize}&\"{fontName}{style}\"{text}";
    }

    #endregion
}