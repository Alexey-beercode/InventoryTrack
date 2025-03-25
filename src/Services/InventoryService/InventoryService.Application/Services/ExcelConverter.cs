using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace InventoryService.Application.Services;

public static class ExcelConverter
{
    public static void ConvertXlsToXlsx(string inputXlsPath, string outputXlsxPath)
    {
        using var fileStream = new FileStream(inputXlsPath, FileMode.Open, FileAccess.Read);
        var hssfWorkbook = new HSSFWorkbook(fileStream); // чтение .xls
        var xssfWorkbook = new XSSFWorkbook();           // новый .xlsx

        for (int i = 0; i < hssfWorkbook.NumberOfSheets; i++)
        {
            var sourceSheet = hssfWorkbook.GetSheetAt(i);
            var targetSheet = xssfWorkbook.CreateSheet(sourceSheet.SheetName);

            for (int rowIndex = sourceSheet.FirstRowNum; rowIndex <= sourceSheet.LastRowNum; rowIndex++)
            {
                var sourceRow = sourceSheet.GetRow(rowIndex);
                if (sourceRow == null) continue;

                var targetRow = targetSheet.CreateRow(rowIndex);
                for (int colIndex = 0; colIndex < sourceRow.LastCellNum; colIndex++)
                {
                    var sourceCell = sourceRow.GetCell(colIndex);
                    if (sourceCell == null) continue;

                    var targetCell = targetRow.CreateCell(colIndex);
                    targetCell.SetCellValue(sourceCell.ToString());
                }
            }
        }

        using var outStream = new FileStream(outputXlsxPath, FileMode.Create, FileAccess.Write);
        xssfWorkbook.Write(outStream);
    }
}