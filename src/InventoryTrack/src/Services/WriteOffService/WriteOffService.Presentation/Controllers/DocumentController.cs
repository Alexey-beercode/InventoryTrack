using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WriteOffService.Application.DTOs.Response.Document;
using WriteOffService.Application.Interfaces.Services;

namespace WriteOffService.Presentation.Controllers
{
    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IDocumentService documentService, 
            ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> UploadDocument(IFormFile file, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Uploading document: {FileName}", file.FileName);
            var document = await _documentService.CreateDocumentAsync(file, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, document.Id);
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