using ProductApi.DTOs;
using ProductApi.Models;
using ProductApi.Repositories;
using ProductApi.Extensions.Mappings;

namespace ProductApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository productRepository;
    private readonly IUnitOfWork unitOfWork;

    public ProductService(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        this.productRepository = productRepository;
        this.unitOfWork = unitOfWork;
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
        var product = request.ToProduct();

        await productRepository.Add(product);
        await unitOfWork.SaveChanges();

        return product;
    }

    public async Task<Product?> Update(
        int id,
        UpdateProductRequest request)
    {
        var product = await productRepository.GetById(id);

        if (product is null)
            return null;

        request.ToProduct(product);

        await productRepository.Update(product);
        await unitOfWork.SaveChanges();

        return product;
    }

    public async Task<bool> Delete(int id)
    {
        var product = await productRepository.GetById(id);

        if (product is null)
            return false;

        await productRepository.Delete(product);
        await unitOfWork.SaveChanges();

        return true;
    }
}