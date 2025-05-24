using ClosedXML.Excel;
using MongoDB.Bson;
using ReportService.Application.Services;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;
using System.Reflection;

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
    public void GenerateStockStateSheet_SingleWarehouse_ShouldGenerateCorrectData()
    {
        // Arrange
        var service = new ExcelExportService();
        var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("StockState");
        
        var data = new BsonDocument
        {
            { "warehouse", "Склад 1" },
            { "items", new BsonArray
                {
                    new BsonDocument
                    {
                        { "name", "Товар А" },
                        { "uniqueCode", "XYZ789" },
                        { "quantity", 5 },
                        { "estimatedValue", 100.25 },
                        { "expirationDate", new BsonDateTime(DateTime.UtcNow.AddMonths(6)) },
                        { "supplier", "Поставщик XYZ" },
                        { "deliveryDate", new BsonDateTime(DateTime.UtcNow.AddDays(-10)) }
                    }
                }
            }
        };

        // Act
        service.GenerateStockStateSheet(worksheet, data);

        // Assert
        Assert.Equal("Склад", worksheet.Cell(1, 1).Value);
        Assert.Equal("Наименование", worksheet.Cell(1, 2).Value);
        Assert.Equal("Товар А", worksheet.Cell(3, 2).Value);
        Assert.Equal(5, worksheet.Cell(3, 4).Value);
        Assert.Equal(100.25m, worksheet.Cell(3, 5).Value);
        Assert.Equal("Поставщик XYZ", worksheet.Cell(3, 7).Value);
    }

    [Fact]
    public void GenerateStockStateSheet_MultiWarehouse_ShouldGenerateCorrectData()
    {
        // Arrange
        var service = new ExcelExportService();
        var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("MultiWarehouse");
        
        var data = new BsonDocument
        {
            { "Warehouses", new BsonArray
                {
                    new BsonDocument
                    {
                        { "Name", "Склад A" },
                        { "Items", new BsonArray
                            {
                                new BsonDocument
                                {
                                    { "Name", "Товар 1" },
                                    { "UniqueCode", "A001" },
                                    { "Quantity", 10 },
                                    { "EstimatedValue", 50.0 },
                                    { "ExpirationDate", new BsonDateTime(DateTime.UtcNow.AddMonths(3)) },
                                    { "Supplier", "Поставщик 1" },
                                    { "DeliveryDate", new BsonDateTime(DateTime.UtcNow.AddDays(-5)) }
                                }
                            }
                        }
                    },
                    new BsonDocument
                    {
                        { "Name", "Склад B" },
                        { "Items", new BsonArray
                            {
                                new BsonDocument
                                {
                                    { "Name", "Товар 2" },
                                    { "UniqueCode", "B002" },
                                    { "Quantity", 15 },
                                    { "EstimatedValue", 75.0 },
                                    { "Supplier", "Поставщик 2" }
                                }
                            }
                        }
                    }
                }
            }
        };

        // Act
        service.GenerateStockStateSheet(worksheet, data);

        // Assert
        Assert.Equal("Склад", worksheet.Cell(1, 1).Value);
        Assert.Equal("Наименование", worksheet.Cell(1, 2).Value);
        Assert.Equal("Склад A", worksheet.Cell(2, 1).Value);
        Assert.Equal("Товар 1", worksheet.Cell(3, 2).Value);
        Assert.Equal("Склад B", worksheet.Cell(4, 1).Value);
        Assert.Equal("Товар 2", worksheet.Cell(5, 2).Value);
        Assert.Equal(15, worksheet.Cell(5, 4).Value);
    }

    [Fact]
    public void GenerateMovementsSheet_ShouldGenerateCorrectData()
    {
        // Arrange
        var service = new ExcelExportService();
        var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Movements");
        
        var data = new BsonDocument
        {
            { "Movements", new BsonArray
                {
                    new BsonDocument
                    {
                        { "ItemName", "Товар для перемещения" },
                        { "SourceWarehouseName", "Склад источник" },
                        { "DestinationWarehouseName", "Склад назначения" },
                        { "Quantity", 7 },
                        { "RequestDate", new BsonDateTime(DateTime.UtcNow.AddDays(-2)) },
                        { "Status", "Выполнено" },
                        { "ResponsiblePerson", "Иванов И.И." }
                    }
                }
            }
        };

        // Act
        service.GetType().GetMethod("GenerateMovementsSheet", 
            BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(service, new object[] { worksheet, data });

        // Assert
        Assert.Equal("Наименование", worksheet.Cell(1, 1).Value);
        Assert.Equal("Склад отправления", worksheet.Cell(1, 2).Value);
        Assert.Equal("Товар для перемещения", worksheet.Cell(2, 1).Value);
        Assert.Equal("Склад источник", worksheet.Cell(2, 2).Value);
        Assert.Equal("Склад назначения", worksheet.Cell(2, 3).Value);
        Assert.Equal(7, worksheet.Cell(2, 4).Value);
        Assert.Equal("Выполнено", worksheet.Cell(2, 6).Value);
        Assert.Equal("Иванов И.И.", worksheet.Cell(2, 7).Value);
    }

    [Fact]
    public void GenerateWriteOffsSheet_ShouldGenerateCorrectData()
    {
        // Arrange
        var service = new ExcelExportService();
        var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("WriteOffs");
        
        var data = new BsonDocument
        {
            { "WriteOffs", new BsonArray
                {
                    new BsonDocument
                    {
                        { "ItemName", "Списанный товар" },
                        { "WarehouseName", "Склад списания" },
                        { "Quantity", 3 },
                        { "Reason", "Истек срок годности" },
                        { "RequestDate", new BsonDateTime(DateTime.UtcNow.AddDays(-1)) },
                        { "ApprovedByUser", "Петров П.П." },
                        { "ResponsiblePerson", "Сидоров С.С." }
                    }
                }
            }
        };

        // Act
        service.GetType().GetMethod("GenerateWriteOffsSheet", 
            BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(service, new object[] { worksheet, data });

        // Assert
        Assert.Equal("Наименование", worksheet.Cell(1, 1).Value);
        Assert.Equal("Склад", worksheet.Cell(1, 2).Value);
        Assert.Equal("Количество", worksheet.Cell(1, 3).Value);
        Assert.Equal("Списанный товар", worksheet.Cell(2, 1).Value);
        Assert.Equal("Склад списания", worksheet.Cell(2, 2).Value);
        Assert.Equal(3, worksheet.Cell(2, 3).Value);
        Assert.Equal("Истек срок годности", worksheet.Cell(2, 4).Value);
        Assert.Equal("Петров П.П.", worksheet.Cell(2, 6).Value);
    }

    [Fact]
    public void GenerateItemsHistorySheet_ShouldGenerateCorrectData()
    {
        // Arrange
        var service = new ExcelExportService();
        var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("ItemsHistory");
        
        var data = new BsonDocument
        {
            { "Items", new BsonArray
                {
                    new BsonDocument
                    {
                        { "Name", "Исторический товар" },
                        { "UniqueCode", "HIST001" },
                        { "Quantity", 10 },
                        { "EstimatedValue", 200.0 },
                        { "DeliveryDate", new BsonDateTime(DateTime.UtcNow.AddMonths(-2)) },
                        { "ExpirationDate", new BsonDateTime(DateTime.UtcNow.AddMonths(6)) },
                        { "Status", "Активен" },
                        { "Supplier", "История поставок" },
                        { "WarehouseDetails", new BsonArray
                            {
                                new BsonDocument
                                {
                                    { "WarehouseId", "WH-HIST" }
                                }
                            }
                        },
                        { "DocumentInfo", "Накладная №12345" }
                    }
                }
            }
        };

        // Act
        service.GetType().GetMethod("GenerateItemsHistorySheet", 
            BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(service, new object[] { worksheet, data });

        // Assert
        Assert.Equal("Наименование", worksheet.Cell(1, 1).Value);
        Assert.Equal("Уникальный код", worksheet.Cell(1, 2).Value);
        Assert.Equal("Файл документа", worksheet.Cell(1, 10).Value);
        Assert.Equal("Исторический товар", worksheet.Cell(2, 1).Value);
        Assert.Equal("HIST001", worksheet.Cell(2, 2).Value);
        Assert.Equal(10, worksheet.Cell(2, 3).Value);
        Assert.Equal(200.0, worksheet.Cell(2, 4).Value);
        Assert.Equal("Активен", worksheet.Cell(2, 7).Value);
        Assert.Equal("WH-HIST", worksheet.Cell(2, 9).Value);
        Assert.Equal("Накладная №12345", worksheet.Cell(2, 10).Value);
    }

    [Fact]
    public void FormatDate_WithValidDate_ShouldReturnFormattedString()
    {
        // Arrange
        var service = new ExcelExportService();
        var testDate = DateTime.UtcNow;
        var bsonDoc = new BsonDocument
        {
            { "testDate", new BsonDateTime(testDate) }
        };
        
        // Act
        var result = service.GetType().GetMethod("FormatDate", 
            BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(service, new object[] { bsonDoc, "testDate" }) as string;
        
        // Assert
        Assert.Equal(testDate.ToString("dd/MM/yyyy"), result);
    }

    [Fact]
    public async Task GenerateExcelReportAsync_WithUnknownReportType_ShouldThrowException()
    {
        // Arrange
        var service = new ExcelExportService();
        var report = new Report
        {
            Name = "Invalid Report",
            ReportType = (ReportType)999, // Несуществующий тип отчета
            Data = new BsonDocument()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
            service.GenerateExcelReportAsync(report));
    }
    
    [Fact]
    public async Task GenerateExcelReportAsync_ShouldGenerateGoodExcelFile()
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
}