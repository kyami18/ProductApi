using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.DTOs;
using ProductApi.Services;
using Microsoft.AspNetCore.RateLimiting;

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
    [EnableRateLimiting("AuthPolicy")]

    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await authService.Login(request);

        if (result is null)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Username hoặc password không đúng",
                Data = null
            });
        }

        return Ok(new ApiResponse<LoginResponse>
        {
            Success = true,
            Message = "Đăng nhập thành công",
            Data = result
        });
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("AuthPolicy")]

    public async Task<IActionResult> Refresh(
        RefreshTokenRequest request)
    {
        var result = await authService.Refresh(
            request.RefreshToken);

        if (result is null)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Refresh Token không hợp lệ hoặc đã hết hạn",
                Data = null
            });
        }

        return Ok(new ApiResponse<LoginResponse>
        {
            Success = true,
            Message = "Làm mới token thành công",
            Data = result
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        RefreshTokenRequest request)
    {
        var result = await authService.Logout(
            request.RefreshToken);

        if (!result)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Refresh Token không hợp lệ",
                Data = null
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Đăng xuất thành công",
            Data = null
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