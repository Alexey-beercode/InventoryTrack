using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WriteOffService.Application.DTOs.Response.WriteOffReason;
using WriteOffService.Application.Interfaces.Services;

namespace WriteOffService.Presentation.Controllers
{
    [ApiController]
    [Route("api/write-off-reasons")]
    public class WriteOffReasonController : ControllerBase
    {
        private readonly IWriteOffReasonService _writeOffReasonService;
        private readonly ILogger<WriteOffReasonController> _logger;

        public WriteOffReasonController(IWriteOffReasonService writeOffReasonService, 
            ILogger<WriteOffReasonController> logger)
        {
            _writeOffReasonService = writeOffReasonService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<WriteOffReasonResponseDto>>> GetAll(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all write-off reasons");
            var reasons = await _writeOffReasonService.GetAllAsync(cancellationToken);
            return Ok(reasons);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WriteOffReasonResponseDto>> GetById(Guid id, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting write-off reason by id: {Id}", id);
            var reason = await _writeOffReasonService.GetByIdAsync(id, cancellationToken);
            if (reason == null)
                return NotFound();
            return Ok(reason);
        }

        [HttpGet("by-name/{name}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WriteOffReasonResponseDto>> GetByName(string name, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting write-off reason by name: {Name}", name);
            var reason = await _writeOffReasonService.GetByNameAsync(name, cancellationToken);
            if (reason == null)
                return NotFound();
            return Ok(reason);
        }

        [HttpPost]
        [Authorize(Policy = "Accountant")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] string name, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new write-off reason with name: {Name}", name);
            await _writeOffReasonService.CreateAsync(name, cancellationToken);
            return CreatedAtAction(nameof(GetByName), new { name }, null);
        }
    }
}