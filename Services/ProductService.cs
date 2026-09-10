using Microsoft.EntityFrameworkCore;
using ProductApi.DTOs;
using ProductApi.Models;
using ProductApi.Repositories;
using System.Xml.Linq;
namespace ProductApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository productRepository;

    public ProductService(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }
    public async Task<List<Product>> GetAll(
        string? name,
        int page,
        int pageSize,
        string? sortBy,
        string? sortOrder)
    {
        return await productRepository.GetAll(
            name,
            page,
            pageSize,
            sortBy,
            sortOrder);
    }
    public async Task<int> Count(string? name)
    {
        return await productRepository.Count(name);
    }

    public async Task<Product?> GetById(int id)
    {
        return await productRepository.GetById(id);
    }
    public async Task<Product> Create(CreateProductRequest request)
    {
        int newId = 1;

        var products = await productRepository.GetAll(null, 1, int.MaxValue,null,null);
        if (products.Any())
        {
            newId = products.Max(p => p.Id) + 1;
        }

        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };

        await productRepository.Add(product);

        return product;
    }
    public async Task<Product?> Update(int id, UpdateProductRequest request)
    {
        var product = await productRepository.GetById(id);

        if (product is null)
            return null;

        product.Name = request.Name;
        product.Price = request.Price;

        await productRepository.Update(product);

        return product;
    }
    public async Task<bool> Delete(int id)
    {
        var product = await productRepository.GetById(id);

        if (product is null)
            return false;

        await productRepository.Delete(product);

        return true;
    }
}