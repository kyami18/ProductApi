using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ProductApi.Configurations;
using ProductApi.Data;
using ProductApi.Extensions;
using ProductApi.Filters;
using ProductApi.Models;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


var corsSettings = builder.Configuration
    .GetSection("CorsSettings")
    .Get<CorsSettings>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(corsSettings!.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var response = new ProductApi.DTOs.ApiResponse<object>
        {
            Success = false,
            Message = "Bạn đã gửi quá nhiều yêu cầu. Vui lòng thử lại sau.",
            Data = null
        };

        await context.HttpContext.Response.WriteAsJsonAsync(
            response,
            cancellationToken);
    };

    options.AddFixedWindowLimiter("AuthPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
        limiterOptions.AutoReplenishment = true;
    });
});
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token theo dạng: Bearer {token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray()
            );

        return new BadRequestObjectResult(
            new ProductApi.DTOs.ApiResponse<object>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ",
                Data = errors
            }
        );
    };
});

builder.Services.AddApplicationConfiguration(
    builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
           new Product
           {
               Id = 1,
               Name = "Laptop",
               Price = 20000000
           },
           new Product
           {
               Id = 2,
               Name = "Mouse",
               Price = 500000
           },
           new Product
           {
               Id = 3,
               Name = "Keyboard",
               Price = 1000000
           }                                              
         );

        db.SaveChanges();
    }

    var passwordHasher = new PasswordHasher<User>();

    var users = db.Users.ToList();

    if (!users.Any())
    {
        var admin = new User
        {
            Id = 1,
            Username = "admin",
            Role = "Admin"
        };

        admin.Password = passwordHasher.HashPassword(admin, "123456");

        var user = new User
        {
            Id = 2,
            Username = "user",
            Role = "User"
        };

        user.Password = passwordHasher.HashPassword(user, "123456");

        db.Users.AddRange(admin, user);
    }
    else
    {
        foreach (var user in users)
        {
            if (user.Password == "123456")
            {
                user.Password = passwordHasher.HashPassword(
                    user,
                    "123456"
                );
            }
        }
    }

    db.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseRateLimiter();

app.UseApplicationMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();