using ProductApi.Models;

namespace ProductApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsername(string username);

    Task<User?> GetByRefreshToken(string refreshToken);

    Task SaveChanges();
}