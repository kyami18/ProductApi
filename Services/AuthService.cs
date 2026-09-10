using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Configurations;
using ProductApi.Data;
using ProductApi.DTOs;
using ProductApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductApi.Services;

public class AuthService : IAuthService
{
    private readonly AuthSettings authSettings;
    private readonly AppDbContext db;
    private readonly PasswordHasher<User> passwordHasher;

    public AuthService(
        IOptions<AuthSettings> authSettings,
        AppDbContext db)
    {
        this.authSettings = authSettings.Value;
        this.db = db;
        this.passwordHasher = new PasswordHasher<User>();
    }

    public async Task<LoginResponse?> Login(LoginRequest request)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null)
        {
            return null;
        }

        var passwordResult = passwordHasher.VerifyHashedPassword(
            user,
            user.Password,
            request.Password
        );

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return await GenerateLoginResponse(user);
    }

    public async Task<LoginResponse?> Refresh(string refreshToken)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user is null)
        {
            return null;
        }

        if (user.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        return await GenerateLoginResponse(user);
    }

    public async Task<bool> Logout(string refreshToken)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user is null)
        {
            return false;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        await db.SaveChangesAsync();

        return true;
    }

    private async Task<LoginResponse> GenerateLoginResponse(User user)
    {
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

        return new LoginResponse
        {
            Token = tokenString,
            RefreshToken = refreshToken,
            Username = user.Username,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }
}