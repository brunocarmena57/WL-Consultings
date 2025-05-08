using Microsoft.AspNetCore.Mvc;
using WLConsultingChallenge.Core.DTOs;
using WLConsultingChallenge.Core.Services;

namespace WLConsulting_Challenge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public AuthController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _userService.RegisterUser(model.Username, model.Email, model.Password);
            
            return Ok(new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userService.GetUserByUsername(model.Username);
        if (user == null)
            return Unauthorized(new { message = "Invalid username or password" });

        var isPasswordValid = _authService.VerifyPassword(model.Password, user.PasswordHash);
        if (!isPasswordValid)
            return Unauthorized(new { message = "Invalid username or password" });

        var token = _authService.GenerateJwtToken(user);

        return Ok(new LoginResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            }
        });
    }
}
