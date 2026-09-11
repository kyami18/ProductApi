using ProductApi.DTOs;
using ProductApi.Models;

namespace ProductApi.Extensions.Mappings;

public static class ProductRequestMappings
{
    public static Product ToProduct(
        this CreateProductRequest request)
    {
        return new Product
        {
            Name = request.Name,
            Price = request.Price
        };
    }
    public static Product ToProduct(
    this UpdateProductRequest request,
    Product product)
    {
        product.Name = request.Name;
        product.Price = request.Price;

        return product;
    }
}