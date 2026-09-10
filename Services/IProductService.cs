using ProductApi.DTOs;
using ProductApi.Models;

namespace ProductApi.Services;

public interface IProductService

{
    Task<List<Product>> GetAll(
        string? name, 
        int page, 
        int pageSize, 
        string? sortBy,
        string? sortOrder);
    Task<int> Count(string? name);
    Task<Product?> GetById(int id);
    Task<Product> Create(CreateProductRequest request);
    Task<Product?> Update(int id, UpdateProductRequest request);
    Task<bool> Delete(int id);
}