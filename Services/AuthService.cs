using System.Text;
using ProductApi.DTOs;
using ProductApi.Models;
using System.Security.Claims;
using ProductApi.Repositories;
using ProductApi.Configurations;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ProductApi.Services;




public class AuthService : IAuthService
{
    private readonly AuthSettings authSettings;
    private readonly IUserRepository userRepository;
    private readonly PasswordHasher<User> passwordHasher;
    private readonly ILogger<AuthService> logger;

    public AuthService(
     IOptions<AuthSettings> authSettings,
     IUserRepository userRepository,
     ILogger<AuthService> logger)
    {
        this.authSettings = authSettings.Value;
        this.userRepository = userRepository;
        this.passwordHasher = new PasswordHasher<User>();
        this.logger = logger;
    }

    public async Task<LoginResponse?> Login(LoginRequest request)
    {
        var user = await userRepository.GetByUsername(
            request.Username);

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
            logger.LogWarning(
                "Login failed for username {Username}: invalid password",
                request.Username);

            return null;
        }

        return await GenerateLoginResponse(user);
    }

    public async Task<LoginResponse?> Refresh(string refreshToken)
    {
        var user = await userRepository.GetByRefreshToken(
            refreshToken);

        if (user is null)
        {
            logger.LogWarning(
                "Refresh token invalid or user not found");

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
        var user = await userRepository.GetByRefreshToken(
            refreshToken);

        if (user is null)
        {
            return false;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        await userRepository.SaveChanges();

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
            RandomNumberGenerator.GetBytes(32));

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

        await userRepository.SaveChanges();

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