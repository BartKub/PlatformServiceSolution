using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        Task<bool> SaveChangesAsync();

        Task<IEnumerable<Platform>> GetAllAsync();
        Task<Platform> GetPlatformByIdAsync(int Id);
        Task CreatePlatformAsync(Platform platform);
    }
}
