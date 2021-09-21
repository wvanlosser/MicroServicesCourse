using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }

        public static void SeedData(AppDbContext context, bool isProduction)
        {
            if(isProduction)
            {
                System.Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate(); 
                }
                catch(Exception ex)
                {
                    System.Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
            }

            if (context.Platforms.Any())
            {
                System.Console.WriteLine("--> We already have data");
                return;
            } 

            System.Console.WriteLine("--> Seeding Data...");

            context.Platforms.AddRange(
                new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free"},
                new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free"},
                new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free"}
            );

            context.SaveChanges();

            System.Console.WriteLine("--> Seeding Data Completed.");
        }
    }
}