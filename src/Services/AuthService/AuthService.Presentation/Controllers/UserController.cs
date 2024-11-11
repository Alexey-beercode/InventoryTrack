using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using AuthService.BLL.DTOs.Implementations.Requests.User;
using AuthService.BLL.DTOs.Implementations.Responses.User;
using AuthService.BLL.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize("Accountant")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserRepsonseDTO>>> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserRepsonseDTO>> GetById(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("by-login/{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserRepsonseDTO>> GetByLogin(string login)
        {
            var result = await _userService.GetByLoginAsync(login);
            return Ok(result);
        }

        [HttpGet("by-name")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserRepsonseDTO>> GetByName([FromBody] GetUserByNameDTO getUserByNameDto)
        {
            var result = await _userService.GetByNameAsync(getUserByNameDto);
            return Ok(result);
        }

        [Authorize("Accountant")]
        [HttpGet("by-company/{companyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserRepsonseDTO>>> GetByCompanyId(Guid companyId)
        {
            var result = await _userService.GetByCompanyIdAsync(companyId);
            return Ok(result);
        }

        [Authorize("Accountant")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> RegisterUserToCompany([FromBody] RegisterUserToCompanyDTO registerUserToCompanyDto)
        {
            await _userService.RegisterUserToCompany(registerUserToCompanyDto);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Update([FromBody] UpdateUserDTO updateUserDto)
        {
            await _userService.UpdateAsync(updateUserDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _userService.DeleteAsync(id);
            return Ok();
        }

        [HttpPost("user-to-company")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddUserToCompanyAsync(
            [FromBody] RegisterUserToCompanyDTO registerUserToCompanyDto, CancellationToken cancellationToken = default)
        {
            await _userService.RegisterUserToCompany(registerUserToCompanyDto, cancellationToken);
            return Ok();
        }
    }
}