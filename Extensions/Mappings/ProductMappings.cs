using ProductApi.DTOs;
using ProductApi.Models;

namespace ProductApi.Extensions.Mappings;

public static class ProductMappings
{
    public static ProductResponse ToResponse(
        this Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
    }
}