namespace ProductApi.DTOs;

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}