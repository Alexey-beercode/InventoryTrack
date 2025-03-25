using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.DTOs.Response.Document;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;
        private readonly IInventoryDocumentService _inventoryDocumentService;

        public DocumentController(IDocumentService documentService, 
            ILogger<DocumentController> logger, IInventoryDocumentService inventoryDocumentService)
        {
            _documentService = documentService;
            _logger = logger;
            _inventoryDocumentService = inventoryDocumentService;
        }

        [Authorize(Policy = "Accountant")]
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateDocument(
            [FromBody] GenerateInventoryDocumentDto dto,
            CancellationToken cancellationToken = default)
        {
                byte[] fileData;
                string fileName;
                string contentType;

                if (dto.IsWriteOff)
                {
                    fileData = await _inventoryDocumentService.GenerateWriteOffDocumentAsync(dto, cancellationToken);
                    fileName = $"Акт о списании {DateTime.Now:dd.MM.yyyy}.docx";
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                }
                else
                {
                    fileData = await _inventoryDocumentService.GenerateMovementDocumentAsync(dto, cancellationToken);
                    fileName = $"ТТН {DateTime.Now:dd.MM.yyyy}.xls";
                    contentType = "application/vnd.ms-excel";
                }

                return File(fileData, contentType, fileName);
        }


        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> UploadDocument([FromBody] DocumentDto documentDto,CancellationToken cancellationToken)
        {
            var document = await _documentService.CreateDocumentAsync(documentDto, cancellationToken);
            return Ok(document.Id);
        }

        [HttpGet("{id:guid}/info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DocumentInfoResponseDto>> GetDocumentInfo(Guid id, 
            CancellationToken cancellationToken)
        {
            var documentInfo = await _documentService.GetDocumentInfoAsync(id, cancellationToken);
            return Ok(documentInfo);
        }

        [HttpGet("{id:guid}/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadDocument(Guid id, 
            CancellationToken cancellationToken)
        {
            var document = await _documentService.GetDocumentAsync(id, cancellationToken);
            
            return File(document.FileData, document.FileType, document.FileName);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDocument(Guid id, 
            CancellationToken cancellationToken)
        {
            await _documentService.DeleteDocumentAsync(id, cancellationToken);
            return NoContent();
        }
    }
}