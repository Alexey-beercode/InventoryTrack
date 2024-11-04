using AuthService.BLL.DTOs.Implementations.Requests.UserRole;
using AuthService.BLL.DTOs.Implementations.Responses.Role;
using AuthService.BLL.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Authorize("Accountant")]
[ApiController]
[Route("api/roles")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoleDTO>>> GetAll()
    {
        var result = await _roleService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RoleDTO>> GetById(Guid id)
    {
        var result = await _roleService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("by-user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRolesByUserId(Guid userId)
    {
        var result = await _roleService.GetRolesByUserIdAsync(userId);
        return Ok(result);
    }

    [HttpPost("assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> SetRoleToUser([FromBody] UserRoleDTO userRoleDto)
    {
        await _roleService.SetRoleToUserAsync(userRoleDto);
        return Ok();
    }

    [HttpPost("remove")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RemoveRoleFromUser([FromBody] UserRoleDTO userRoleDto)
    {
        await _roleService.RemoveRoleFromUserAsync(userRoleDto);
        return Ok();
    }
}