using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                    throw new ArgumentOutOfRangeException($"Unknown report type: {report.ReportType}");
            }

            // Сохраняем документ в массив байтов
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void GenerateStockStateSheet(IXLWorksheet worksheet, BsonDocument data)
        {
            var items = data["Items"].AsBsonArray;

            // Заголовки
            worksheet.Cell(1, 1).Value = "Наименование";
            worksheet.Cell(1, 2).Value = "Уникальный код";
            worksheet.Cell(1, 3).Value = "Количество";
            worksheet.Cell(1, 4).Value = "Оценочная стоимость";
            worksheet.Cell(1, 5).Value = "Срок годности";
            worksheet.Cell(1, 6).Value = "Поставщик";
            worksheet.Cell(1, 7).Value = "Склад";
            worksheet.Cell(1, 8).Value = "Дата поступления";

            int row = 2;
            foreach (var item in items)
            {
                var bsonItem = item.AsBsonDocument;
                worksheet.Cell(row, 1).Value = bsonItem["Name"].AsString; // Наименование
                worksheet.Cell(row, 2).Value = bsonItem["UniqueCode"].AsString; // Уникальный код
                worksheet.Cell(row, 3).Value = bsonItem["Quantity"].AsInt64; // Количество
                worksheet.Cell(row, 4).Value = bsonItem["EstimatedValue"].AsDecimal; // Оценочная стоимость
                worksheet.Cell(row, 5).Value = bsonItem["ExpirationDate"].ToUniversalTime(); // Срок годности
                worksheet.Cell(row, 6).Value = bsonItem["Supplier"]["Name"].AsString; // Поставщик
                worksheet.Cell(row, 7).Value = bsonItem["Warehouse"]["Name"].AsString; // Склад
                worksheet.Cell(row, 8).Value = bsonItem["DeliveryDate"].ToUniversalTime(); // Дата поступления
                row++;
            }

            // Автоматическое форматирование столбцов
            worksheet.Columns().AdjustToContents();
        }


        private void GenerateMovementsSheet(IXLWorksheet worksheet, BsonDocument data)
        {
            var items = data["Movements"].AsBsonArray;

            worksheet.Cell(1, 1).Value = "Наименование";
            worksheet.Cell(1, 2).Value = "Склад отправления";
            worksheet.Cell(1, 3).Value = "Склад назначения";
            worksheet.Cell(1, 4).Value = "Количество";
            worksheet.Cell(1, 5).Value = "Дата запроса";
            worksheet.Cell(1, 6).Value = "Статус";

            int row = 2;
            foreach (var item in items)
            {
                var bsonItem = item.AsBsonDocument;
                worksheet.Cell(row, 1).Value = bsonItem["Name"].AsString; // Наименование
                worksheet.Cell(row, 2).Value = bsonItem["SourceWarehouse"]["Name"].AsString; // Склад отправления
                worksheet.Cell(row, 3).Value = bsonItem["DestinationWarehouse"]["Name"].AsString; // Склад назначения
                worksheet.Cell(row, 4).Value = bsonItem["Quantity"].AsInt64; // Количество
                worksheet.Cell(row, 5).Value = bsonItem["RequestDate"].ToUniversalTime(); // Дата запроса
                worksheet.Cell(row, 6).Value = bsonItem["Status"]["Name"].AsString; // Статус
                row++;
            }

            worksheet.Columns().AdjustToContents();
        }


        private void GenerateWriteOffsSheet(IXLWorksheet worksheet, BsonDocument data)
        {
            var items = data["WriteOffs"].AsBsonArray;

            worksheet.Cell(1, 1).Value = "Наименование";
            worksheet.Cell(1, 2).Value = "Склад";
            worksheet.Cell(1, 3).Value = "Количество";
            worksheet.Cell(1, 4).Value = "Причина";
            worksheet.Cell(1, 5).Value = "Дата запроса";
            worksheet.Cell(1, 6).Value = "Утверждено пользователем";

            int row = 2;
            foreach (var item in items)
            {
                var bsonItem = item.AsBsonDocument;
                worksheet.Cell(row, 1).Value = bsonItem["Name"].AsString; // Наименование
                worksheet.Cell(row, 2).Value = bsonItem["Warehouse"]["Name"].AsString; // Склад
                worksheet.Cell(row, 3).Value = bsonItem["Quantity"].AsInt64; // Количество
                worksheet.Cell(row, 4).Value = bsonItem["Reason"]["Reason"].AsString; // Причина
                worksheet.Cell(row, 5).Value = bsonItem["RequestDate"].ToUniversalTime(); // Дата запроса
                worksheet.Cell(row, 6).Value = bsonItem["ApprovedByUserId"].ToString(); // Утверждено пользователем
                row++;
            }

            worksheet.Columns().AdjustToContents();
        }


        private void GenerateItemsHistorySheet(IXLWorksheet worksheet, BsonDocument data)
        {
            var items = data["History"].AsBsonArray;

            worksheet.Cell(1, 1).Value = "Тип операции";
            worksheet.Cell(1, 2).Value = "Наименование";
            worksheet.Cell(1, 3).Value = "Склад";
            worksheet.Cell(1, 4).Value = "Количество";
            worksheet.Cell(1, 5).Value = "Дата";

            int row = 2;
            foreach (var item in items)
            {
                var bsonItem = item.AsBsonDocument;
                worksheet.Cell(row, 1).Value = bsonItem["OperationType"].AsString; // Тип операции
                worksheet.Cell(row, 2).Value = bsonItem["Name"].AsString; // Наименование
                worksheet.Cell(row, 3).Value = bsonItem["Warehouse"]["Name"].AsString; // Склад
                worksheet.Cell(row, 4).Value = bsonItem["Quantity"].AsInt64; // Количество
                worksheet.Cell(row, 5).Value = bsonItem["Date"].ToUniversalTime(); // Дата
                row++;
            }

            worksheet.Columns().AdjustToContents();
        }

    }
}
