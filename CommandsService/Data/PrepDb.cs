using System.Collections.Generic;
using CommandService.SyncDataServices;
using CommandsService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var platformClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
                var platforms = platformClient.RetrieveAllPlatforms();

                SeedData(serviceScope.ServiceProvider.GetRequiredService<ICommandRepo>(), platforms);
            }
        }

        private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            System.Console.WriteLine("--> Seeding new platforms ...");

            foreach (var platform in platforms)
            {
                if (!repo.ExternalPlatformExists(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                }
            }
            repo.SaveChanges();
        }
    }
}