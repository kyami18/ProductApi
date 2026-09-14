using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductApi.DTOs;

namespace ProductApi.Tests.Integration;

public class ProductsApiTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public ProductsApiTests(
        WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetProducts_Should_Return_Success()
    {
        // Arrange
        var client = factory.CreateClient();

        var loginRequest = new LoginRequest
        {
            Username = "user",
            Password = "123456"
        };

        var loginResponse = await client.PostAsJsonAsync(
            "/api/Auth/login",
            loginRequest);

        loginResponse.EnsureSuccessStatusCode();

        var loginResult =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult!.Token);

        // Act
        var response = await client.GetAsync("/api/Products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task GetProductById_Should_Return_Success()
    {
        // Arrange
        var client = factory.CreateClient();

        var loginRequest = new LoginRequest
        {
            Username = "user",
            Password = "123456"
        };

        var loginResponse = await client.PostAsJsonAsync(
            "/api/Auth/login",
            loginRequest);

        loginResponse.EnsureSuccessStatusCode();

        var loginResult =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult!.Token);

        // Act
        var response = await client.GetAsync("/api/Products/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task GetProductById_Should_Return_NotFound_When_Product_Does_Not_Exist()
    {
        // Arrange
        var client = factory.CreateClient();

        var loginRequest = new LoginRequest
        {
            Username = "user",
            Password = "123456"
        };

        var loginResponse = await client.PostAsJsonAsync(
            "/api/Auth/login",
            loginRequest);

        loginResponse.EnsureSuccessStatusCode();

        var loginResult =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult!.Token);

        // Act
        var response = await client.GetAsync("/api/Products/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task CreateProduct_Should_Return_Created_When_Admin()
    {
        // Arrange
        var client = factory.CreateClient();

        var loginRequest = new LoginRequest
        {
            Username = "admin",
            Password = "123456"
        };

        var loginResponse = await client.PostAsJsonAsync(
            "/api/Auth/login",
            loginRequest);

        loginResponse.EnsureSuccessStatusCode();

        var loginResult =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult!.Token);

        var request = new
        {
            Name = "Integration Test Product",
            Price = 123456
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/Products",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    [Fact]
    public async Task CreateProduct_Should_Return_Forbidden_When_User()
    {
        // Arrange
        var client = factory.CreateClient();

        var loginRequest = new LoginRequest
        {
            Username = "user",
            Password = "123456"
        };

        var loginResponse = await client.PostAsJsonAsync(
            "/api/Auth/login",
            loginRequest);

        loginResponse.EnsureSuccessStatusCode();

        var loginResult =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult!.Token);

        var request = new
        {
            Name = "Unauthorized Product",
            Price = 100000
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/Products",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}