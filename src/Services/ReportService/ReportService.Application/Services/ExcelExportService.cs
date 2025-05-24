using ClosedXML.Excel;
using MongoDB.Bson;
using ReportService.Domain.Entities;
using ReportService.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ReportService.Application.Services
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly ILogger<ExcelExportService> _logger;

        public ExcelExportService(ILogger<ExcelExportService> logger)
        {
            _logger = logger;
        }
        
        // ✅ УЛУЧШЕННЫЙ метод форматирования даты
        private string FormatDate(BsonDocument doc, string fieldName)
        {
            // Проверяем все возможные варианты названий полей
            var possibleFields = new[] { fieldName, fieldName.ToLower(), ToPascalCase(fieldName), ToCamelCase(fieldName) };
            
            foreach (var field in possibleFields)
            {
                if (doc.Contains(field))
                {
                    var value = doc[field];
                    _logger.LogInformation("🔍 Найдено поле {Field} с типом {Type} и значением {Value}", 
                        field, value.BsonType, value);
                    
                    try
                    {
                        switch (value.BsonType)
                        {
                            case BsonType.DateTime:
                                return value.ToUniversalTime().ToString("dd/MM/yyyy");
                            case BsonType.String:
                                if (DateTime.TryParse(value.AsString, out var parsedDate))
                                {
                                    return parsedDate.ToString("dd/MM/yyyy");
                                }
                                return value.AsString;
                            case BsonType.Null:
                                return "";
                            default:
                                return value.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Ошибка при форматировании даты для поля {Field}", field);
                        return "";
                    }
                }
            }
            
            _logger.LogWarning("⚠ Поле {FieldName} не найдено в документе. Доступные поля: {Fields}", 
                fieldName, string.Join(", ", doc.Names));
            return "";
        }

        // Вспомогательные методы для конвертации регистра
        private string ToPascalCase(string input) => char.ToUpper(input[0]) + input.Substring(1);
        private string ToCamelCase(string input) => char.ToLower(input[0]) + input.Substring(1);

        // ✅ УЛУЧШЕННЫЙ метод получения значения
        private string GetStringValue(BsonDocument doc, string fieldName)
        {
            var possibleFields = new[] { fieldName, fieldName.ToLower(), ToPascalCase(fieldName), ToCamelCase(fieldName) };
            
            foreach (var field in possibleFields)
            {
                if (doc.Contains(field) && !doc[field].IsBsonNull)
                {
                    return doc[field].AsString;
                }
            }
            return "";
        }

        private int GetIntValue(BsonDocument doc, string fieldName)
        {
            var possibleFields = new[] { fieldName, fieldName.ToLower(), ToPascalCase(fieldName), ToCamelCase(fieldName) };
            
            foreach (var field in possibleFields)
            {
                if (doc.Contains(field) && !doc[field].IsBsonNull)
                {
                    return doc[field].ToInt32();
                }
            }
            return 0;
        }

        private decimal GetDecimalValue(BsonDocument doc, string fieldName)
        {
            var possibleFields = new[] { fieldName, fieldName.ToLower(), ToPascalCase(fieldName), ToCamelCase(fieldName) };
            
            foreach (var field in possibleFields)
            {
                if (doc.Contains(field) && !doc[field].IsBsonNull)
                {
                    return doc[field].ToDecimal();
                }
            }
            return 0M;
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

            _logger.LogInformation("🔍 Структура данных: {Keys}", string.Join(", ", data.Names));

            // 1️⃣ Проверяем тип данных: Warehouses или единичный склад
            if (data.Contains("Warehouses") && data["Warehouses"].BsonType == BsonType.Array)
            {
                GenerateMultiWarehouseSheet(worksheet, data["Warehouses"].AsBsonArray);
            }
            else if (data.Contains("warehouses") && data["warehouses"].BsonType == BsonType.Array)
            {
                GenerateMultiWarehouseSheet(worksheet, data["warehouses"].AsBsonArray);
            }
            else if (data.Contains("items") && data["items"].BsonType == BsonType.Array && data.Contains("warehouse"))
            {
                GenerateSingleWarehouseSheet(worksheet, data["items"].AsBsonArray, GetStringValue(data, "warehouse"));
            }
            else if (data.Contains("Items") && data["Items"].BsonType == BsonType.Array && data.Contains("Warehouse"))
            {
                GenerateSingleWarehouseSheet(worksheet, data["Items"].AsBsonArray, GetStringValue(data, "Warehouse"));
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
                
                _logger.LogInformation("🔍 Обработка товара: {Fields}", string.Join(", ", bsonItem.Names));

                worksheet.Cell(row, 1).Value = warehouseName;
                worksheet.Cell(row, 2).Value = GetStringValue(bsonItem, "name");
                worksheet.Cell(row, 3).Value = GetStringValue(bsonItem, "uniqueCode");
                worksheet.Cell(row, 4).Value = GetIntValue(bsonItem, "quantity");
                worksheet.Cell(row, 5).Value = GetDecimalValue(bsonItem, "estimatedValue");

                // ✅ ИСПРАВЛЕНО: унифицированное получение дат
                worksheet.Cell(row, 6).Value = FormatDate(bsonItem, "expirationDate");
                worksheet.Cell(row, 7).Value = GetStringValue(bsonItem, "supplier");
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
                string warehouseName = GetStringValue(bsonWarehouse, "name");

                _logger.LogInformation("🔍 Обработка склада: {WarehouseName}, поля: {Fields}", 
                    warehouseName, string.Join(", ", bsonWarehouse.Names));

                worksheet.Cell(row, 1).Value = warehouseName;
                worksheet.Range(row, 1, row, 8).Merge().Style.Font.Bold = true;
                worksheet.Range(row, 1, row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                row++;

                // Проверяем все возможные варианты названия поля с товарами
                BsonArray? items = null;
                if (bsonWarehouse.Contains("Items") && bsonWarehouse["Items"].BsonType == BsonType.Array)
                {
                    items = bsonWarehouse["Items"].AsBsonArray;
                }
                else if (bsonWarehouse.Contains("items") && bsonWarehouse["items"].BsonType == BsonType.Array)
                {
                    items = bsonWarehouse["items"].AsBsonArray;
                }

                if (items == null)
                {
                    worksheet.Cell(row, 2).Value = "Нет товаров";
                    row++;
                    continue;
                }

                foreach (var item in items)
                {
                    var bsonItem = item.AsBsonDocument;
                    
                    _logger.LogInformation("🔍 Обработка товара в складе: поля={Fields}", 
                        string.Join(", ", bsonItem.Names));

                    worksheet.Cell(row, 1).Value = warehouseName; 
                    worksheet.Cell(row, 2).Value = GetStringValue(bsonItem, "name");
                    worksheet.Cell(row, 3).Value = GetStringValue(bsonItem, "uniqueCode");
                    worksheet.Cell(row, 4).Value = GetIntValue(bsonItem, "quantity");
                    worksheet.Cell(row, 5).Value = GetDecimalValue(bsonItem, "estimatedValue");
            
                    // ✅ ИСПРАВЛЕНО: унифицированное получение дат
                    worksheet.Cell(row, 6).Value = FormatDate(bsonItem, "expirationDate");
                    worksheet.Cell(row, 7).Value = GetStringValue(bsonItem, "supplier");
                    worksheet.Cell(row, 8).Value = FormatDate(bsonItem, "deliveryDate");

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
            worksheet.Cell(1, 7).Value = "Ответственный за операцию";

            int row = 2;
            foreach (var movement in movements)
            {
                var bsonMovement = movement.AsBsonDocument;

                worksheet.Cell(row, 1).Value = GetStringValue(bsonMovement, "ItemName") ?? "Неизвестный товар";
                worksheet.Cell(row, 2).Value = GetStringValue(bsonMovement, "SourceWarehouseName") ?? "Неизвестный склад";
                worksheet.Cell(row, 3).Value = GetStringValue(bsonMovement, "DestinationWarehouseName") ?? "Неизвестный склад";
                worksheet.Cell(row, 4).Value = GetIntValue(bsonMovement, "Quantity");
                worksheet.Cell(row, 5).Value = FormatDate(bsonMovement, "RequestDate");
                worksheet.Cell(row, 6).Value = GetStringValue(bsonMovement, "Status") ?? "Неизвестный статус";
                worksheet.Cell(row, 7).Value = GetStringValue(bsonMovement, "ResponsiblePerson") ?? "Неизвестный пользователь";
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
            worksheet.Cell(1, 7).Value = "Ответственный за операцию";

            int row = 2;
            foreach (var writeOff in writeOffs)
            {
                var bsonWriteOff = writeOff.AsBsonDocument;
                
                worksheet.Cell(row, 1).Value = GetStringValue(bsonWriteOff, "ItemName");
                worksheet.Cell(row, 2).Value = GetStringValue(bsonWriteOff, "WarehouseName");
                worksheet.Cell(row, 3).Value = GetIntValue(bsonWriteOff, "Quantity");
                worksheet.Cell(row, 4).Value = GetStringValue(bsonWriteOff, "Reason");
                worksheet.Cell(row, 5).Value = FormatDate(bsonWriteOff, "RequestDate");
                worksheet.Cell(row, 6).Value = GetStringValue(bsonWriteOff, "ApprovedByUser") ?? "Не утверждено";
                worksheet.Cell(row, 7).Value = GetStringValue(bsonWriteOff, "ResponsiblePerson") ?? "Неизвестный пользователь";

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
                
                worksheet.Cell(row, 1).Value = GetStringValue(bsonItem, "Name");
                worksheet.Cell(row, 2).Value = GetStringValue(bsonItem, "UniqueCode");
                worksheet.Cell(row, 3).Value = GetIntValue(bsonItem, "Quantity");
                worksheet.Cell(row, 4).Value = GetDecimalValue(bsonItem, "EstimatedValue");
                worksheet.Cell(row, 5).Value = FormatDate(bsonItem, "DeliveryDate");
                worksheet.Cell(row, 6).Value = FormatDate(bsonItem, "ExpirationDate");
                worksheet.Cell(row, 7).Value = GetStringValue(bsonItem, "Status");
                worksheet.Cell(row, 8).Value = GetStringValue(bsonItem, "Supplier");

                // Получение информации о складе (если он есть в массиве)
                if (bsonItem.Contains("WarehouseDetails") && bsonItem["WarehouseDetails"].BsonType == BsonType.Array)
                {
                    var warehouseDetails = bsonItem["WarehouseDetails"].AsBsonArray;
                    worksheet.Cell(row, 9).Value = warehouseDetails.Count > 0 
                        ? GetStringValue(warehouseDetails[0].AsBsonDocument, "WarehouseId")
                        : "Нет данных";
                }
                else
                {
                    worksheet.Cell(row, 9).Value = "Нет данных";
                }

                // Запись информации о документе (если есть)
                worksheet.Cell(row, 10).Value = GetStringValue(bsonItem, "DocumentInfo");

                row++;
            }

            worksheet.Columns().AdjustToContents();
        }
    }
}