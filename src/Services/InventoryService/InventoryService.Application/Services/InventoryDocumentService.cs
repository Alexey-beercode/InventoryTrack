using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Application.Services;
using InventoryService.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using TemplateEngine.Docx;

public class InventoryDocumentService : IInventoryDocumentService
{
    private readonly IInventoryItemRepository _itemRepo;
    private readonly IWarehouseRepository _warehouseRepo;
    private readonly string _writeOffTemplatePath;
    private readonly string _movementTemplatePath;

    public InventoryDocumentService(
        IInventoryItemRepository itemRepo,
        IWarehouseRepository warehouseRepo,
        IConfiguration config)
    {
        _itemRepo = itemRepo;
        _warehouseRepo = warehouseRepo;

        _writeOffTemplatePath = config["DocumentTemplates:WriteOff"]; // путь к шаблону
        _movementTemplatePath = config["DocumentTemplates:Movement"];
    }

    public async Task<byte[]> GenerateWriteOffDocumentAsync(GenerateInventoryDocumentDto dto, CancellationToken cancellationToken)
    {
        var item = await _itemRepo.GetByIdAsync(dto.InventoryItemId, cancellationToken);
        var warehouse = await _warehouseRepo.GetByIdAsync(dto.WarehouseId, cancellationToken);

        using var templateStream = File.OpenRead(_writeOffTemplatePath);
        using var mem = new MemoryStream();
        await templateStream.CopyToAsync(mem, cancellationToken);
        mem.Position = 0;

        using (var wordDoc = WordprocessingDocument.Open(mem, true))
        {
            var allTexts = wordDoc.MainDocumentPart.Document.Descendants<Text>();

            foreach (var text in allTexts)
            {
                if (string.IsNullOrWhiteSpace(text.Text)) continue;

                text.Text = text.Text
                    .Replace("«наименование»", item.Name)
                    .Replace("«единица»", "шт")
                    .Replace("«количество»", dto.Quantity.ToString())
                    .Replace("«цена»", item.EstimatedValue.ToString("F2"))
                    .Replace("«сумма»", (item.EstimatedValue * dto.Quantity).ToString("F2"))
                    .Replace("«номер»", "1");
            }

            wordDoc.MainDocumentPart.Document.Save();
        }

        return mem.ToArray();
    }


public async Task<byte[]> GenerateMovementDocumentAsync(GenerateInventoryDocumentDto dto, CancellationToken cancellationToken)
{
    var item = await _itemRepo.GetByIdAsync(dto.InventoryItemId, cancellationToken);
    var source = await _warehouseRepo.GetByIdAsync(dto.SourceWarehouseId, cancellationToken);
    var destination = await _warehouseRepo.GetByIdAsync(dto.WarehouseId, cancellationToken);

    await using var templateStream = new FileStream(_movementTemplatePath, FileMode.Open, FileAccess.Read);
    var workbook = new HSSFWorkbook(templateStream); // Работаем с .xls
    var sheet = workbook.GetSheetAt(0); // Предполагаем, что нужный лист — первый

    // 🔁 Обход всех ячеек и замена плейсхолдеров
    for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
    {
        var row = sheet.GetRow(i);
        if (row == null) continue;

        for (int j = 0; j < row.LastCellNum; j++)
        {
            var cell = row.GetCell(j);
            if (cell == null || cell.CellType != NPOI.SS.UserModel.CellType.String) continue;

            var text = cell.StringCellValue;

            // Замены
            text = text.Replace("{{наименование}}", item.Name)
                       .Replace("{{единица}}", "шт")
                       .Replace("{{количество}}", dto.Quantity.ToString())
                       .Replace("{{цена}}", item.EstimatedValue.ToString("F2"))
                       .Replace("{{стоимость}}", (item.EstimatedValue * dto.Quantity).ToString("F2"))
                       .Replace("{{грузоотправитель}}", source.Name)
                       .Replace("{{грузополучатель}}", destination.Name)
                       .Replace("{{адрес отправителя}}", source.Location ?? "-")
                       .Replace("{{адрес получателя}}", destination.Location ?? "-");

            cell.SetCellValue(text);
        }
    }

    // 💾 Сохраняем результат
    await using var outputStream = new MemoryStream();
    workbook.Write(outputStream);
    return outputStream.ToArray();
}


}
