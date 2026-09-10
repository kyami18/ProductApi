using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;

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
        return await db.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }

    public async Task SaveChanges()
    {
        await db.SaveChangesAsync();
    }
}