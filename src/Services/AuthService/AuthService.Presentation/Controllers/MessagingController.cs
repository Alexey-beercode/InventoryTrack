using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthService.BLL.Interfaces.Services;

namespace AuthService.Controllers;

[ApiController]
[Route("api/auth/messaging")]
public class MessagingController : ControllerBase
{
    private readonly IUserService _userService;

    public MessagingController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users/{id:guid}")]
    public async Task<ActionResult> GetUserDetails(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user == null) return NotFound();

        var response = new
        {
            user.Id,
            user.Login,
            user.FirstName,
            user.LastName
        };

        return Ok(response);
    }
}