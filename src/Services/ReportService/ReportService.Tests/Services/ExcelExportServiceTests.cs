using ClosedXML.Excel;
using MongoDB.Bson;
using ReportService.Application.Services;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;

namespace ReportService.Tests.Services;

public class ExcelExportServiceTests
{
    [Fact]
    public async Task GenerateExcelReportAsync_ShouldGenerateExcelFile()
    {
        // Arrange
        var service = new ExcelExportService();
        var report = new Report
        {
            Name = "Test Report",
            ReportType = ReportType.StockState,
            Data = new BsonDocument
            {
                { "Items", new BsonArray
                    {
                        new BsonDocument
                        {
                            { "Name", "Item 1" },
                            { "UniqueCode", "ABC123" },
                            { "Quantity", BsonValue.Create(10) }, // Универсально для Int32/Int64
                            { "EstimatedValue", 50.5 },
                            { "ExpirationDate", BsonDateTime.Create("2024-12-31T00:00:00Z") },
                            { "Supplier", new BsonDocument { { "Name", "Supplier A" } } },
                            { "Warehouse", new BsonDocument { { "Name", "Warehouse 1" } } },
                            { "DeliveryDate", BsonDateTime.Create("2024-01-01T00:00:00Z") }
                        }
                    }
                }
            }
        };

        // Act
        var result = await service.GenerateExcelReportAsync(report);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);

        // Проверим, что результат является Excel-файлом
        using var stream = new MemoryStream(result);
        var workbook = new XLWorkbook(stream);
        Assert.NotNull(workbook.Worksheets.Worksheet("Test Report"));
    }
    
    [Fact]
    public async Task GenerateExcelReportAsync_ShouldThrowExceptionForInvalidReportType()
    {
        // Arrange
        var service = new ExcelExportService();
        var report = new Report
        {
            Name = "Invalid Report",
            ReportType = (ReportType)999, // Неверный тип
            Data = new BsonDocument()
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GenerateExcelReportAsync(report));
        Assert.Contains("Unknown report type", exception.Message);
    }

    [Fact]
    public void GenerateStockStateSheet_ShouldPopulateWorksheetCorrectly()
    {
        // Arrange
        var service = new ExcelExportService();
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Test");
        var data = new BsonDocument
        {
            { "Items", new BsonArray
                {
                    new BsonDocument
                    {
                        { "Name", "Item 1" },
                        { "UniqueCode", "ABC123" },
                        { "Quantity", 10 },
                        { "EstimatedValue", 50.5 },
                        { "ExpirationDate", BsonDateTime.Create("2024-12-31T00:00:00Z") },
                        { "Supplier", new BsonDocument { { "Name", "Supplier A" } } },
                        { "Warehouse", new BsonDocument { { "Name", "Warehouse 1" } } },
                        { "DeliveryDate", BsonDateTime.Create("2024-01-01T00:00:00Z") }
                    }
                }
            }
        };

        // Act
        service.GenerateStockStateSheet(worksheet, data);

        // Assert
        Assert.Equal("Наименование", worksheet.Cell(1, 1).Value);
        Assert.Equal("Item 1", worksheet.Cell(2, 1).Value);
        Assert.Equal(10, worksheet.Cell(2, 3).Value);
        Assert.Equal("Supplier A", worksheet.Cell(2, 6).Value);
    }

}