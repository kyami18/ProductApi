using ProductApi.Models;

namespace ProductApi.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAll(
        string? name,
        int page,
        int pageSize,
        string? sortBy,
        string? sortOrder);

    Task<int> Count(string? name);

    Task<Product?> GetById(int id);

    Task Add(Product product);

    Task Update(Product product);

    Task Delete(Product product);
}