using ClosedXML.Excel;
using MongoDB.Bson;
using ReportService.Domain.Entities;
using ReportService.Domain.Interfaces.Services;

namespace ReportService.Application.Services
{
    public class ExcelExportService : IExcelExportService
    {
        
        private string FormatDate(BsonDocument doc, string fieldName)
        {
            if (doc.Contains(fieldName) && doc[fieldName].BsonType == BsonType.DateTime)
            {
                return doc[fieldName].ToUniversalTime().ToString("dd/MM/yyyy");
            }
            return "";
        }
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
            worksheet.Cell(1, 1).Value = "Склад";
            worksheet.Cell(1, 2).Value = "Наименование";
            worksheet.Cell(1, 3).Value = "Уникальный код";
            worksheet.Cell(1, 4).Value = "Количество";
            worksheet.Cell(1, 5).Value = "Оценочная стоимость";
            worksheet.Cell(1, 6).Value = "Срок годности";
            worksheet.Cell(1, 7).Value = "Поставщик";
            worksheet.Cell(1, 8).Value = "Дата поступления";

            // 1️⃣ Проверяем тип данных: Warehouses или единичный склад
            if (data.Contains("Warehouses") && data["Warehouses"].BsonType == BsonType.Array)
            {
                GenerateMultiWarehouseSheet(worksheet, data["Warehouses"].AsBsonArray);
            }
            else if (data.Contains("items") && data["items"].BsonType == BsonType.Array && data.Contains("warehouse"))
            {
                GenerateSingleWarehouseSheet(worksheet, data["items"].AsBsonArray, data["warehouse"].AsString);
            }
            else
            {
                worksheet.Cell(2, 1).Value = "❌ Данные отсутствуют или формат не распознан";
                worksheet.Range(2, 1, 2, 8).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            worksheet.Columns().AdjustToContents();
        }

        private void GenerateSingleWarehouseSheet(IXLWorksheet worksheet, BsonArray items, string warehouseName)
        {
            int row = 2;

            worksheet.Cell(row, 1).Value = warehouseName;
            worksheet.Range(row, 1, row, 8).Merge().Style.Font.Bold = true;
            worksheet.Range(row, 1, row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            row++;

            foreach (var item in items)
            {
                var bsonItem = item.AsBsonDocument;

                worksheet.Cell(row, 1).Value = warehouseName;
                worksheet.Cell(row, 2).Value = bsonItem.GetValue("name", "").AsString;
                worksheet.Cell(row, 3).Value = bsonItem.GetValue("uniqueCode", "").AsString;
                worksheet.Cell(row, 4).Value = bsonItem.GetValue("quantity", 0).ToInt32();
                worksheet.Cell(row, 5).Value = bsonItem.GetValue("estimatedValue", 0.0M).ToDecimal();

                worksheet.Cell(row, 6).Value = FormatDate(bsonItem, "expirationDate");
                worksheet.Cell(row, 7).Value = bsonItem.GetValue("supplier", "").AsString;
                worksheet.Cell(row, 8).Value = FormatDate(bsonItem, "deliveryDate");

                row++;
            }
        }

        private void GenerateMultiWarehouseSheet(IXLWorksheet worksheet, BsonArray warehouses)
        {
            int row = 2;

            foreach (var warehouse in warehouses)
            {
                var bsonWarehouse = warehouse.AsBsonDocument;
                string warehouseName = bsonWarehouse.GetValue("Name", "").AsString;

                worksheet.Cell(row, 1).Value = warehouseName;
                worksheet.Range(row, 1, row, 8).Merge().Style.Font.Bold = true;
                worksheet.Range(row, 1, row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                row++;

                if (!bsonWarehouse.Contains("Items") || bsonWarehouse["Items"].BsonType != BsonType.Array)
                {
                    worksheet.Cell(row, 2).Value = "Нет товаров";
                    row++;
                    continue;
                }

                var items = bsonWarehouse["Items"].AsBsonArray;
                foreach (var item in items)
                {
                    var bsonItem = item.AsBsonDocument;

                    worksheet.Cell(row, 1).Value = warehouseName; 
                    worksheet.Cell(row, 2).Value = bsonItem.GetValue("Name", "").AsString;
                    worksheet.Cell(row, 3).Value = bsonItem.GetValue("UniqueCode", "").AsString;
                    worksheet.Cell(row, 4).Value = bsonItem.GetValue("Quantity", 0).ToInt32();
                    worksheet.Cell(row, 5).Value = bsonItem.GetValue("EstimatedValue", 0.0M).ToDecimal();
            
                    worksheet.Cell(row, 6).Value = FormatDate(bsonItem, "ExpirationDate");
                    worksheet.Cell(row, 7).Value = bsonItem.GetValue("Supplier", "").AsString;
                    worksheet.Cell(row, 8).Value = FormatDate(bsonItem, "DeliveryDate");

                    row++;
                }
            }
        }



private void GenerateMovementsSheet(IXLWorksheet worksheet, BsonDocument data)
{
    if (!data.Contains("Movements") || data["Movements"].IsBsonNull)
    {
        worksheet.Cell(1, 1).Value = "Данные отсутствуют";
        worksheet.Range(1, 1, 1, 6).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Columns().AdjustToContents();
        return;
    }

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

        worksheet.Cell(row, 1).Value = bsonMovement.GetValue("ItemName", "Неизвестный товар").AsString;
        worksheet.Cell(row, 2).Value = bsonMovement.GetValue("SourceWarehouseName", "Неизвестный склад").AsString;
        worksheet.Cell(row, 3).Value = bsonMovement.GetValue("DestinationWarehouseName", "Неизвестный склад").AsString;
        worksheet.Cell(row, 4).Value = bsonMovement.GetValue("Quantity", 0).ToInt32();

        // Форматирование даты запроса
        worksheet.Cell(row, 5).Value = bsonMovement.Contains("RequestDate") && bsonMovement["RequestDate"].BsonType != BsonType.Null
            ? bsonMovement["RequestDate"].ToUniversalTime().ToString("dd/MM/yyyy")
            : "";

        worksheet.Cell(row, 6).Value = bsonMovement.GetValue("Status", "Неизвестный статус").AsString;
        row++;
    }

    worksheet.Columns().AdjustToContents();
}




private void GenerateWriteOffsSheet(IXLWorksheet worksheet, BsonDocument data)
{
    if (!data.Contains("WriteOffs") || data["WriteOffs"].IsBsonNull)
    {
        worksheet.Cell(1, 1).Value = "Данные отсутствуют";
        worksheet.Range(1, 1, 1, 6).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Columns().AdjustToContents();
        return;
    }

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
        worksheet.Cell(row, 6).Value = bsonWriteOff.GetValue("ApprovedByUser", "Не утверждено").AsString; // ✅ Фикс

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
