using ProductApi.DTOs;

namespace ProductApi.Services;

public interface IAuthService
{
    Task<LoginResponse?> Login(LoginRequest request);

    Task<LoginResponse?> Refresh(string refreshToken);

    Task<bool> Logout(string refreshToken);
}