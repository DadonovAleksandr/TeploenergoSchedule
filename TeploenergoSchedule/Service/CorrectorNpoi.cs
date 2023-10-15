using NLog;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TeploenergoSchedule.Model.FileInfo;

namespace TeploenergoSchedule.Service;

internal class CorrectorNpoi
{
    private static Logger _log = LogManager.GetCurrentClassLogger();
    private readonly Regex regexYearOfApproval = new Regex(@"^[_"" ]*(20)\d{2}\s*г");
    private readonly Regex regexYearOfImplementation = new Regex(@"\sна\s(20)\d{2}\s*г");
    private readonly Regex regexYear = new Regex(@"(20)\d{2}");
    private readonly string _yearOfApproval;
    private readonly string _yearOfImplementation;

    public CorrectorNpoi(string yearOfApproval, string yearOfImplementation)
    {
        _yearOfApproval = yearOfApproval;
        _yearOfImplementation = yearOfImplementation;
    }

    public bool Correct(FileState fileState)
    {
        _log.Debug($"Корректировка файла: \"{fileState}\"");
        fileState.State = FileStateEnum.Processing;

        if (!File.Exists(fileState.Path))
        {
            _log.Error($"Файл \"{fileState}\" не существует");
            fileState.State = FileStateEnum.Error;
            throw new FileNotFoundException();
        }

        IWorkbook workbook;
        try
        {
            using (FileStream fs = new FileStream(fileState.Path, FileMode.Open, FileAccess.Read))
            {
                var fileExt = Path.GetExtension(fs.Name);


                workbook = fileExt switch
                {
                    ".xls" => new HSSFWorkbook(fs),
                    ".xlsx" => new XSSFWorkbook(fs),
                    _ => new XSSFWorkbook(fs),

                };
            }

            _log.Debug($"Открываем файл \"{fileState}\"");
            var sheetsCount = workbook.NumberOfSheets;
            var successCount = 0;
            for (int i = 0; i < sheetsCount; i++)
            {
                if (SheetCorrect(workbook.GetSheetAt(i)))
                    successCount++;
            }

            if (successCount == sheetsCount)
                fileState.State = FileStateEnum.CorrectedSuccess;
            else
                fileState.State = FileStateEnum.CorrectedWithWarning;

            using (FileStream fs = new FileStream(fileState.Path, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            return true;
        }
        catch(Exception ex)
        {
            _log.Error(ex.Message);
            fileState.State = FileStateEnum.Error;
            return false;
        }

    }

    private bool SheetCorrect(ISheet sheet)
    {
        var rowStart = sheet.FirstRowNum;
        var rowEnd = sheet.LastRowNum;

        _log.Debug($"Корректировка листа \"{sheet.SheetName}\" в диапазоне строк: [{rowStart}] - [{rowEnd}]");
        var yearOfApprovalCount = 0;
        var yearOfImplementationCount = 0;
        for (int row = rowStart; row < rowEnd; row++)
        {
            IRow rowData = sheet.GetRow(row);
            if (rowData == null) continue;
            if (rowData.Cells.All(d => d.CellType == CellType.Blank)) continue;
            for (int col = rowData.FirstCellNum; col < rowData.LastCellNum; col++)
            {
                if (rowData.GetCell(col) != null)
                {
                    if (!string.IsNullOrEmpty(rowData.GetCell(col).ToString()) && !string.IsNullOrWhiteSpace(rowData.GetCell(col).ToString()))
                    {
                        var data = rowData.GetCell(col)?.ToString() ?? string.Empty;
                        _log.Trace($"[{row},{col}]:\"{data}\"");
                        //if (regexYear.IsMatch(data))
                        //    _log.Debug($"[{row},{col}]:\"{data}\" - содержит год");
                        if (regexYearOfApproval.IsMatch(data))
                        {
                            _log.Debug($"[{row},{col}]:\"{data}\" - содержит год утверждения");
                            var oldYear = regexYear.Match(regexYearOfApproval.Match(data).Value).Value;
                            rowData.GetCell(col).SetCellValue(data.Replace(oldYear, _yearOfApproval));
                            yearOfApprovalCount++;
                        }
                        if (regexYearOfImplementation.IsMatch($"{data}"))
                        {
                            _log.Debug($"[{row},{col}]:\"{data}\" - содержит год выполнения");
                            var oldYear = regexYear.Match(regexYearOfImplementation.Match(data).Value).Value;
                            rowData.GetCell(col).SetCellValue(data.Replace(oldYear, _yearOfImplementation));
                            yearOfImplementationCount++;
                        }
                    }
                }
            }
        }

        if (yearOfApprovalCount == 1 && yearOfImplementationCount == 2)
        {
            _log.Debug($"Корректировка листа \"{sheet.SheetName}\" выполнена успешно");
            return true;
        }

        _log.Warn($"Корректировка листа \"{sheet.SheetName}\" выполнена некорректно");
        return false;
    }
}
