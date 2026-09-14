using ProductApi.DTOs;
using ProductApi.Validators;

namespace ProductApi.Tests.Validators;

public class CreateProductRequestValidatorTests
{
    [Fact]
    public async Task Should_Fail_When_Name_Is_Empty()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();

        var request = new CreateProductRequest
        {
            Name = "",
            Price = 100000
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
    }
    [Fact]
    public async Task Should_Fail_When_Price_Is_Zero()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();

        var request = new CreateProductRequest
        {
            Name = "Laptop",
            Price = 0
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
    }
    [Fact]
    public async Task Should_Pass_When_Request_Is_Valid()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();

        var request = new CreateProductRequest
        {
            Name = "Laptop",
            Price = 20000000
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
}