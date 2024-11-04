using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using AuthService.BLL.DTOs.Implementations.Responses.Auth;
using AuthService.BLL.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthReponseDTO>> Register([FromBody] RegisterDTO registerDto)
        {
            _logger.LogInformation("Registering user with login: {Login}", registerDto.Login);

            var result = await _authService.RegisterAsync(registerDto);
            _logger.LogInformation("User with login {Login} registered successfully", registerDto.Login);
            return Ok(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthReponseDTO>> Login([FromBody] LoginDTO loginDto)
        {
            _logger.LogInformation("Attempting login for user with login: {Login}", loginDto.Login);

            var result = await _authService.LoginAsync(loginDto);
            _logger.LogInformation("User with login {Login} logged in successfully", loginDto.Login);
            return Ok(result);
        }
    }
}