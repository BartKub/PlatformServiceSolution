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
        public static void PrepPopulation(IApplicationBuilder appBuilder, bool isProd)
        {
             using var serviceScope = appBuilder.ApplicationServices.CreateScope();

             SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("Applying migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Could not run migrations {e.Message}");
                }
            }

            if (context.Platforms.Any()) return;

            Console.WriteLine("Seeding Data");
            context.Platforms.AddRange(
                new Platform
                {
                    Name = "Dot Net",
                    Publisher = "Microsoft",
                    Cost = "Free",
                },
                new Platform
                {
                    Name = "SQL Express",
                    Publisher = "Microsoft",
                    Cost = "Free",
                },
                new Platform
                {
                    Name = "Kubernetes",
                    Publisher = "Google",
                    Cost = "Free",
                });
            context.SaveChanges();
        }
    }
}
