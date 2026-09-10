using System.Text;
using ProductApi.Data;
using ProductApi.DTOs;
using ProductApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;


namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthSettings authSettings;
    private readonly AppDbContext db;
    private readonly PasswordHasher<User> passwordHasher;

    public AuthController(
        IOptions<AuthSettings> authSettings,
        AppDbContext db)
    {
        this.authSettings = authSettings.Value;
        this.db = db;
        this.passwordHasher = new PasswordHasher<User>();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await db.Users
            .Where(u => u.Username == request.Username)
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Username hoặc password không đúng"
            });
        }

        var passwordResult = passwordHasher.VerifyHashedPassword(
            user,
            user.Password,
            request.Password
        );

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                message = "Username hoặc password không đúng"
            });
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(authSettings.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(
            authSettings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: authSettings.Issuer,
            audience: authSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        var refreshToken = Convert.ToBase64String(
    Guid.NewGuid().ToByteArray());
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

        await db.SaveChangesAsync();

        return Ok(new LoginResponse
        {
            Token = tokenString,
            RefreshToken = refreshToken,
            Username = user.Username,
            Role = user.Role,
            ExpiresAt = expiresAt
        });
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Refresh Token không hợp lệ"
            });
        }

        if (user.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized(new
            {
                message = "Refresh Token đã hết hạn"
            });
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(authSettings.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(
            authSettings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: authSettings.Issuer,
            audience: authSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        var newRefreshToken = Convert.ToBase64String(
           Guid.NewGuid().ToByteArray());

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

        await db.SaveChangesAsync();

        return Ok(new LoginResponse
        {
            Token = tokenString,
            RefreshToken = newRefreshToken,
            Username = user.Username,
            Role = user.Role,
            ExpiresAt = expiresAt
        });
    }
        [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Refresh Token không hợp lệ"
            });
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        await db.SaveChangesAsync();

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
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            username,
            role
        });
    }
}