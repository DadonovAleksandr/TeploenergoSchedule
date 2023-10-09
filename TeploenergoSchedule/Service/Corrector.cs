using OfficeOpenXml;
using System.IO;

namespace TeploenergoSchedule.Service;

internal class Corrector
{
    public bool Correct(string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        if (!File.Exists(filePath))
            throw new FileNotFoundException();

        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            worksheet.Cells[1,1].Value = "1234567";

            package.Save();
        }

        return true;
    }


}