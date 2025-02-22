using ClosedXML.Excel;
using MongoDB.Bson;
using ReportService.Domain.Entities;
using ReportService.Domain.Interfaces.Services;

namespace ReportService.Application.Services
{
    public class ExcelExportService : IExcelExportService
    {
        public async Task<byte[]> GenerateExcelReportAsync(Report report)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(report.Name);

            // Устанавливаем заголовки на основе типа отчета
            switch (report.ReportType)
            {
                case Domain.Enums.ReportType.StockState:
                    GenerateStockStateSheet(worksheet, report.Data);
                    break;
                case Domain.Enums.ReportType.Movements:
                    GenerateMovementsSheet(worksheet, report.Data);
                    break;
                case Domain.Enums.ReportType.WriteOffs:
                    GenerateWriteOffsSheet(worksheet, report.Data);
                    break;
                case Domain.Enums.ReportType.Items:
                    GenerateItemsHistorySheet(worksheet, report.Data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Неизвестный тип отчета: {report.ReportType}");
            }

            // Сохраняем документ в массив байтов
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public void GenerateStockStateSheet(IXLWorksheet worksheet, BsonDocument data)
{
    // Устанавливаем заголовки
    worksheet.Cell(1, 1).Value = "Наименование";
    worksheet.Cell(1, 2).Value = "Уникальный код";
    worksheet.Cell(1, 3).Value = "Количество";
    worksheet.Cell(1, 4).Value = "Оценочная стоимость";
    worksheet.Cell(1, 5).Value = "Срок годности";
    worksheet.Cell(1, 6).Value = "Поставщик";
    worksheet.Cell(1, 7).Value = "Склад";
    worksheet.Cell(1, 8).Value = "Дата поступления";

    // Проверяем, существует ли поле "Items"
    if (!data.Contains("Items") || data["Items"].IsBsonNull)
    {
        worksheet.Cell(2, 1).Value = "Данные отсутствуют";
        worksheet.Range(2, 1, 2, 8).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Columns().AdjustToContents();
        return;
    }

    // Получаем массив "Items"
    var items = data["Items"].AsBsonArray;

    // Если массив пуст, добавляем сообщение
    if (!items.Any())
    {
        worksheet.Cell(2, 1).Value = "Данные отсутствуют";
        worksheet.Range(2, 1, 2, 8).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Columns().AdjustToContents();
        return;
    }

    // Заполняем строки данными
    int row = 2;
    foreach (var item in items)
    {
        var bsonItem = item.AsBsonDocument;
        worksheet.Cell(row, 1).Value = bsonItem.GetValue("Name", "").AsString; // Наименование
        worksheet.Cell(row, 2).Value = bsonItem.GetValue("UniqueCode", "").AsString; // Уникальный код
        worksheet.Cell(row, 3).Value = bsonItem.GetValue("Quantity", 0).ToInt32(); // Количество
        worksheet.Cell(row, 4).Value = bsonItem.GetValue("EstimatedValue", 0.0M).ToDecimal(); // Оценочная стоимость
        worksheet.Cell(row, 5).Value = bsonItem.GetValue("ExpirationDate", BsonNull.Value).ToNullableUniversalTime(); // Срок годности
        worksheet.Cell(row, 6).Value = bsonItem.GetValue("SupplierName", "").AsString; // Поставщик
        worksheet.Cell(row, 7).Value = bsonItem["WarehouseDetails"][0]["WarehouseName"].AsString; 
        worksheet.Cell(row, 8).Value = bsonItem.GetValue("DeliveryDate", BsonNull.Value).ToNullableUniversalTime(); // Дата поступления
        row++;
    }

    worksheet.Columns().AdjustToContents();
}


        private void GenerateMovementsSheet(IXLWorksheet worksheet, BsonDocument data)
        {
            var movements = data["Movements"].AsBsonArray;

            worksheet.Cell(1, 1).Value = "Наименование";
            worksheet.Cell(1, 2).Value = "Склад отправления";
            worksheet.Cell(1, 3).Value = "Склад назначения";
            worksheet.Cell(1, 4).Value = "Количество";
            worksheet.Cell(1, 5).Value = "Дата запроса";
            worksheet.Cell(1, 6).Value = "Статус";

            int row = 2;
            foreach (var movement in movements)
            {
                var bsonMovement = movement.AsBsonDocument;
                worksheet.Cell(row, 1).Value = bsonMovement["Name"].AsString;
                worksheet.Cell(row, 2).Value = bsonMovement["SourceWarehouseId"].AsString; // Склад отправления
                worksheet.Cell(row, 3).Value = bsonMovement["DestinationWarehouseId"].AsString; // Склад назначения
                worksheet.Cell(row, 4).Value = bsonMovement["Quantity"].ToInt32(); // Количество
                worksheet.Cell(row, 5).Value = bsonMovement["RequestDate"].ToUniversalTime(); // Дата запроса
                worksheet.Cell(row, 6).Value = bsonMovement["Status"].AsString; // Статус
                row++;
            }

            worksheet.Columns().AdjustToContents();
        }

        private void GenerateWriteOffsSheet(IXLWorksheet worksheet, BsonDocument data)
        {
            var writeOffs = data["WriteOffs"].AsBsonArray;

            worksheet.Cell(1, 1).Value = "Наименование";
            worksheet.Cell(1, 2).Value = "Склад";
            worksheet.Cell(1, 3).Value = "Количество";
            worksheet.Cell(1, 4).Value = "Причина";
            worksheet.Cell(1, 5).Value = "Дата запроса";
            worksheet.Cell(1, 6).Value = "Утверждено пользователем";

            int row = 2;
            foreach (var writeOff in writeOffs)
            {
                var bsonWriteOff = writeOff.AsBsonDocument;
                worksheet.Cell(row, 1).Value = bsonWriteOff["ItemName"].AsString;
                worksheet.Cell(row, 2).Value = bsonWriteOff["WarehouseName"].AsString;
                worksheet.Cell(row, 3).Value = bsonWriteOff["Quantity"].ToInt32();
                worksheet.Cell(row, 4).Value = bsonWriteOff["Reason"].AsString;
                worksheet.Cell(row, 5).Value = bsonWriteOff["RequestDate"].ToUniversalTime();
                worksheet.Cell(row, 6).Value = bsonWriteOff["ApprovedByUser"].AsString;
                row++;
            }

            worksheet.Columns().AdjustToContents();
        }

        private void GenerateItemsHistorySheet(IXLWorksheet worksheet, BsonDocument data)
        {
            var history = data["Items"].AsBsonArray;

            worksheet.Cell(1, 1).Value = "Наименование";
            worksheet.Cell(1, 2).Value = "Уникальный код";
            worksheet.Cell(1, 3).Value = "Количество";
            worksheet.Cell(1, 4).Value = "Оценочная стоимость";
            worksheet.Cell(1, 5).Value = "Дата доставки";
            worksheet.Cell(1, 6).Value = "Дата истечения срока";
            worksheet.Cell(1, 7).Value = "Статус";
            worksheet.Cell(1, 8).Value = "Поставщик";
            worksheet.Cell(1, 9).Value = "Склад";
            worksheet.Cell(1, 10).Value = "Файл документа";

            int row = 2;
            foreach (var item in history)
            {
                var bsonItem = item.AsBsonDocument;
                worksheet.Cell(row, 1).Value = bsonItem["Name"].AsString;
                worksheet.Cell(row, 2).Value = bsonItem["UniqueCode"].AsString;
                worksheet.Cell(row, 3).Value = bsonItem["Quantity"].ToInt32();
                worksheet.Cell(row, 4).Value = bsonItem["EstimatedValue"].ToDecimal();
                worksheet.Cell(row, 5).Value = bsonItem["DeliveryDate"].ToUniversalTime();
                worksheet.Cell(row, 6).Value = bsonItem["ExpirationDate"].ToUniversalTime();
                worksheet.Cell(row, 7).Value = bsonItem["Status"].AsString;
                worksheet.Cell(row, 8).Value = bsonItem["Supplier"].AsString;

                // Получение информации о складе (если он есть в массиве)
                var warehouseDetails = bsonItem["WarehouseDetails"].AsBsonArray;
                worksheet.Cell(row, 9).Value = warehouseDetails.Count > 0 
                    ? warehouseDetails[0].AsBsonDocument["WarehouseId"].AsString 
                    : "Нет данных";

                // Запись информации о документе (если есть)
                worksheet.Cell(row, 10).Value = bsonItem["DocumentInfo"].AsString;

                row++;
            }

            worksheet.Columns().AdjustToContents();
        }

    }
}
