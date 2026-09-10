using System.ComponentModel.DataAnnotations;

namespace ProductApi.DTOs;

public class UpdateProductRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
};