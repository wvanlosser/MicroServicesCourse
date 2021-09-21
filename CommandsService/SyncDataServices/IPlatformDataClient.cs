using System.Collections.Generic;
using CommandsService.Models;

namespace CommandService.SyncDataServices
{
    public interface IPlatformDataClient
    {
        IEnumerable<Platform> RetrieveAllPlatforms();
    }
}