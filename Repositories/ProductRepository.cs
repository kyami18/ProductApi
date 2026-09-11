using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext db;
    public ProductRepository(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<List<Product>> GetAll(
        string? name,
        int page,
        int pageSize,
        string? sortBy,
        string? sortOrder)
    {
        var query = db.Products.AsQueryable();

        // Tìm kiếm theo tên
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }

        // Sắp xếp
        if (string.Equals(sortBy, "price", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(p => p.Price);
            }
            else
            {
                query = query.OrderBy(p => p.Price);
            }
        }
        else if (string.Equals(sortBy, "name", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(p => p.Name);
            }
            else
            {
                query = query.OrderBy(p => p.Name);
            }
        }
        else
        {
            // Mặc định sắp xếp theo Id
            query = query.OrderBy(p => p.Id);
        }

        // Phân trang
        query = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return await query.ToListAsync();
    }

    public async Task<int> Count(string? name)
    {
        var query = db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }

        return await query.CountAsync();
    }

    public async Task<Product?> GetById(int id)
    {
        return await db.Products
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task Add(Product product)
    {
        db.Products.Add(product);
    }

    public async Task Update(Product product)
    {
        db.Products.Update(product);
    }

    public async Task Delete(Product product)
    {
        db.Products.Remove(product);
    }
}