using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.DTOs;
using ProductApi.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace ProductApi.Controllers;
/// <summary>
/// Cung cấp các API xác thực người dùng và quản lý token.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }
    /// <summary>
    /// Đăng nhập và cấp Access Token cùng Refresh Token.
    /// </summary>
    /// <param name="request">Thông tin đăng nhập.</param>
    /// <returns>Thông tin token nếu đăng nhập thành công.</returns>

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
    /// <summary>
    /// Làm mới Access Token bằng Refresh Token.
    /// </summary>
    /// <param name="request">Refresh Token hiện tại.</param>
    /// <returns>Access Token và Refresh Token mới.</returns>
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
    /// <summary>
    /// Đăng xuất và vô hiệu hóa Refresh Token.
    /// </summary>
    /// <param name="request">Refresh Token cần vô hiệu hóa.</param>
    /// <returns>Kết quả đăng xuất.</returns>
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
    /// <summary>
    /// Lấy thông tin người dùng hiện tại từ Access Token.
    /// </summary>
    /// <returns>Username và Role của người dùng hiện tại.</returns>
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