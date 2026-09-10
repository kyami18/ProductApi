using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Configurations;
using ProductApi.Data;
using ProductApi.DTOs;
using ProductApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

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

        return Ok(new LoginResponse
        {
            Token = tokenString,
            Username = user.Username,
            Role = user.Role,
            ExpiresAt = expiresAt
        });
    }
}