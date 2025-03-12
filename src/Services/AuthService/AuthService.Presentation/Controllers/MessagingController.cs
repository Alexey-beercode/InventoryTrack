using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthService.BLL.Interfaces.Services;

namespace AuthService.Controllers;

[ApiController]
[Route("api/auth/messaging")]
public class MessagingController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<MessagingController> _logger;

    public MessagingController(IUserService userService, ILogger<MessagingController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("users/{id:guid}")]
    public async Task<ActionResult> GetUserDetails(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Id : {id}",id);
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