using NLog;
using OfficeOpenXml;
using System.IO;

namespace TeploenergoSchedule.Service;

internal class Corrector
{
    private static Logger _log = LogManager.GetCurrentClassLogger();

    public bool Correct(string filePath)
    {
        _log.Debug($"Корректировка файла: \"{filePath}\"");
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        if (!File.Exists(filePath))
        {
            _log.Error($"Файл \"{filePath}\" не существует");
            throw new FileNotFoundException();
        }
            
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            _log.Debug($"Открываем файл \"{filePath}\"");
            foreach(var worksheet in package.Workbook.Worksheets)
            {
                var rowStart = worksheet.Dimension.Start.Row;  
                var colStart = worksheet.Dimension.Start.Column;  
                var rowEnd = worksheet.Dimension.End.Row;
                var colEnd = worksheet.Dimension.End.Column;

                _log.Debug($"Корректировка листа \"{worksheet.Name}\" в диапазоне: [{rowStart},{colStart}] - [{rowEnd},{colEnd}]");

                for (int row = rowStart; row < rowEnd; row++)
                {
                    for(int col = colStart; col < colEnd; col++)
                    {
                        var data = worksheet.Cells[row, col].Value;
                        _log.Trace($"[{row},{col}]:\"{data}\"");
                    }
                }
            }
            package.Save();
        }

        return true;
    }


}