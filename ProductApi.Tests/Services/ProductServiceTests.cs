using Moq;
using ProductApi.DTOs;
using ProductApi.Models;
using ProductApi.Repositories;
using ProductApi.Services;

namespace ProductApi.Tests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task Create_Should_Create_Product_Successfully()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var service = new ProductService(
            productRepository.Object,
            unitOfWork.Object);

        var request = new CreateProductRequest
        {
            Name = "Laptop",
            Price = 20000000
        };

        // Act
        var result = await service.Create(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Laptop", result.Name);
        Assert.Equal(20000000, result.Price);

        productRepository.Verify(
            x => x.Add(It.IsAny<Product>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }
    [Fact]
    public async Task Create_Should_Pass_Correct_Product_To_Repository()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        Product? addedProduct = null;

        productRepository
            .Setup(x => x.Add(It.IsAny<Product>()))
            .Callback<Product>(product => addedProduct = product);

        var service = new ProductService(
            productRepository.Object,
            unitOfWork.Object);

        var request = new CreateProductRequest
        {
            Name = "Mouse",
            Price = 500000
        };

        // Act
        await service.Create(request);

        // Assert
        Assert.NotNull(addedProduct);
        Assert.Equal("Mouse", addedProduct.Name);
        Assert.Equal(500000, addedProduct.Price);
    }
    
    
    [Fact]
    public async Task GetById_Should_Return_Null_When_Product_Does_Not_Exist()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(x => x.GetById(999))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        var result = await service.GetById(999);

        // Assert
        Assert.Null(result);

        productRepository.Verify(
            x => x.GetById(999),
            Times.Once);
    }
    [Fact]
    public async Task Delete_Should_Delete_Product_When_Product_Exists()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 20000000
        };

        productRepository
            .Setup(x => x.GetById(1))
            .ReturnsAsync(product);

        var service = new ProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        var result = await service.Delete(1);

        // Assert
        Assert.True(result);

        productRepository.Verify(
            x => x.GetById(1),
            Times.Once);

        productRepository.Verify(
            x => x.Delete(product),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }
    [Fact]
    public async Task Delete_Should_Return_False_When_Product_Does_Not_Exist()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(x => x.GetById(999))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        var result = await service.Delete(999);

        // Assert
        Assert.False(result);

        productRepository.Verify(
            x => x.GetById(999),
            Times.Once);

        productRepository.Verify(
            x => x.Delete(It.IsAny<Product>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChanges(),
            Times.Never);
    }
    [Fact]
    public async Task Update_Should_Update_Product_When_Product_Exists()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 20000000
        };

        productRepository
            .Setup(x => x.GetById(1))
            .ReturnsAsync(product);

        var service = new ProductService(
            productRepository.Object,
            unitOfWork.Object);

        var request = new UpdateProductRequest
        {
            Name = "Gaming Laptop",
            Price = 30000000
        };

        // Act
        var result = await service.Update(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Gaming Laptop", result.Name);
        Assert.Equal(30000000, result.Price);

        productRepository.Verify(
            x => x.GetById(1),
            Times.Once);

        productRepository.Verify(
            x => x.Update(product),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }
    [Fact]
    public async Task Update_Should_Return_Null_When_Product_Not_Exists()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(x => x.GetById(999))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            productRepository.Object,
            unitOfWork.Object);

        var request = new UpdateProductRequest
        {
            Name = "Gaming Laptop",
            Price = 30000000
        };

        // Act
        var result = await service.Update(999, request);

        // Assert
        Assert.Null(result);

        productRepository.Verify(
            x => x.GetById(999),
            Times.Once);

        productRepository.Verify(
            x => x.Update(It.IsAny<Product>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChanges(),
            Times.Never);
    }
    [Fact]
    public async Task Delete_Should_Return_True_When_Product_Exists()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 20000000
        };

        productRepository
            .Setup(x => x.GetById(1))
            .ReturnsAsync(product);

        var service = new ProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        var result = await service.Delete(1);

        // Assert
        Assert.True(result);

        productRepository.Verify(
            x => x.GetById(1),
            Times.Once);

        productRepository.Verify(
            x => x.Delete(product),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }
    [Fact]
    public async Task Delete_Should_Return_False_When_Product_Not_Found()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(x => x.GetById(999))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        var result = await service.Delete(999);

        // Assert
        Assert.False(result);

        productRepository.Verify(
            x => x.GetById(999),
            Times.Once);

        productRepository.Verify(
            x => x.Delete(It.IsAny<Product>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChanges(),
            Times.Never);
    }
    [Fact]
    public async Task GetById_Should_Return_Product_When_Product_Exists()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 20000000
        };

        productRepository
            .Setup(x => x.GetById(1))
            .ReturnsAsync(product);

        var service = new ProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        var result = await service.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Laptop", result.Name);
        Assert.Equal(20000000, result.Price);

        productRepository.Verify(
            x => x.GetById(1),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChanges(),
            Times.Never);
    }
}