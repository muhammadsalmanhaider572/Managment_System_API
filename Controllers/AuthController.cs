using Managment_System_API_Application.DTOs.Auth;
using Managment_System_API_Application.Interfaces;
using Managment_System_Application.DTOs.Auth;
using Managment_System_Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Managment_System_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // =====================================================
    // LOGIN
    // =====================================================

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        var result =
            await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }
}