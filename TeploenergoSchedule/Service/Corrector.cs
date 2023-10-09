using NLog;
using OfficeOpenXml;
using System.IO;
using System.Text.RegularExpressions;

namespace TeploenergoSchedule.Service;

internal class Corrector
{
    private static Logger _log = LogManager.GetCurrentClassLogger();
    private readonly Regex regexYearOfApproval = new Regex(@"_+(20)\d{2}\s+г");
    private readonly Regex regexYearOfImplementation = new Regex(@"\sна\s(20)\d{2}\s+г");
    private readonly Regex regexYear = new Regex(@"(20)\d{2}");
    private readonly string _yearOfApproval;
    private readonly string _yearOfImplementation;

    public Corrector(string yearOfApproval, string yearOfImplementation)
    {
        _yearOfApproval = yearOfApproval;
        _yearOfImplementation = yearOfImplementation;
    }

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
                var yearOfApprovalCount = 0;
                var yearOfImplementationCount = 0;
                for (int row = rowStart; row < rowEnd; row++)
                {
                    for(int col = colStart; col < colEnd; col++)
                    {
                        var data = worksheet.Cells[row, col].Value?.ToString() ?? string.Empty;
                        _log.Trace($"[{row},{col}]:\"{data}\"");
                        if(regexYearOfApproval.IsMatch(data))
                        {
                            _log.Debug($"[{row},{col}]:\"{data}\" - содержит год утверждения");
                            var oldYear = regexYear.Match(regexYearOfApproval.Match(data).Value).Value;
                            worksheet.Cells[row, col].Value = data.Replace(oldYear, _yearOfApproval);
                            yearOfApprovalCount++;
                        }
                        if (regexYearOfImplementation.IsMatch($"{data}"))
                        {
                            _log.Debug($"[{row},{col}]:\"{data}\" - содержит год выполнения");
                            var oldYear = regexYear.Match(regexYearOfImplementation.Match(data).Value).Value;
                            worksheet.Cells[row, col].Value = data.Replace(oldYear, _yearOfImplementation);
                            yearOfImplementationCount++;
                        }
                    }
                }
                if(yearOfApprovalCount == 1 && yearOfImplementationCount == 2) 
                {
                    _log.Debug($"Корректировка листа \"{worksheet.Name}\" выполнена успешно");
                }
                else
                {
                    _log.Warn($"Корректировка листа \"{worksheet.Name}\" выполнена некорректно");
                }
            }
            package.Save();
        }

        return true;
    }


}