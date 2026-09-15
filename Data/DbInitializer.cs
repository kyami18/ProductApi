using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext db)
    {
        db.Database.Migrate();

        SeedProducts(db);
        SeedUsers(db);

        db.SaveChanges();
    }

    private static void SeedProducts(AppDbContext db)
    {
        if (db.Products.Any())
            return;

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
    }

    private static void SeedUsers(AppDbContext db)
    {
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

            admin.Password = passwordHasher.HashPassword(
                admin,
                "123456"
            );

            var user = new User
            {
                Id = 2,
                Username = "user",
                Role = "User"
            };

            user.Password = passwordHasher.HashPassword(
                user,
                "123456"
            );

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
    }
}