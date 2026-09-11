using ProductApi.Repositories;
using ProductApi.Services;

namespace ProductApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthService, AuthService>();
    }
}