using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;
using ProductApi.Services;


namespace ProductApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext db;

    public UserRepository(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await db.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByRefreshToken(string refreshToken)
    {
        var hashedToken = RefreshTokenHasher.Hash(refreshToken);

        return await db.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == hashedToken);
    }
}