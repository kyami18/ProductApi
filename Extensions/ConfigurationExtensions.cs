using ProductApi.Configurations;

namespace ProductApi.Extensions;

public static class ConfigurationExtensions
{
    public static void AddApplicationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ProductSettings>(
            configuration.GetSection("ProductSettings"));
    }
}