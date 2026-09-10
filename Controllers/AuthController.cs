using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.DTOs;
using ProductApi.Services;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await authService.Login(request);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Username hoặc password không đúng"
            });
        }

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        RefreshTokenRequest request)
    {
        var result = await authService.Refresh(
            request.RefreshToken);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Refresh Token không hợp lệ hoặc đã hết hạn"
            });
        }

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        RefreshTokenRequest request)
    {
        var result = await authService.Logout(
            request.RefreshToken);

        if (!result)
        {
            return Unauthorized(new
            {
                message = "Refresh Token không hợp lệ"
            });
        }

        return Ok(new
        {
            message = "Đăng xuất thành công"
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var username = User.Identity?.Name;
        var role = User.FindFirst(
            System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new
        {
            username,
            role
        });
    }
}