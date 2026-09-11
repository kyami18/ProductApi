using ProductApi.Data;

namespace ProductApi.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext db;

    public UnitOfWork(AppDbContext db)
    {
        this.db = db;
    }

    public async Task SaveChanges()
    {
        await db.SaveChangesAsync();
    }
}