using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Configurations;

namespace ProductApi.Extensions;

public static class AuthenticationExtensions
{
    public static void AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuthSettings>(
            configuration.GetSection("AuthSettings"));

        var authSettings = configuration
            .GetSection("AuthSettings")
            .Get<AuthSettings>();

        services.AddAuthentication(
            JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = authSettings!.Issuer,
                        ValidAudience = authSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    authSettings.SecretKey))
                    };
            });

        services.AddAuthorization();
    }
}