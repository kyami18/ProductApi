using FluentValidation;
using ProductApi.Repositories;
using ProductApi.Services;
using ProductApi.Validators;

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
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddValidatorsFromAssemblyContaining<
            CreateProductRequestValidator>();
    }
}