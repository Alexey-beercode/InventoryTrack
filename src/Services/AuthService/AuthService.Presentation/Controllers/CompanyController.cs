using AuthService.BLL.DTOs.Implementations.Requests.Company;
using AuthService.BLL.DTOs.Implementations.Responses.Company;
using AuthService.BLL.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Authorize("Accountant")]
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CompanyResponseDTO>> GetById(Guid id)
        {
            var result = await _companyService.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> Create([FromBody] CreateCompanyDTO companyDto)
        {
            await _companyService.CreateAsync(companyDto);
            return Created();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Update([FromBody] UpdateCompanyDTO companyDto)
        {
            await _companyService.UpdateAsync(companyDto);
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CompanyResponseDTO>>> GetAll()
        {
            var result = await _companyService.GetAllAsync();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _companyService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("by-user-id/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var company = await _companyService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(company);
        }
        
    }
}