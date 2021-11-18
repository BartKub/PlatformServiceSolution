using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo: IPlatformRepo
    {
        private readonly AppDbContext _context;

        public PlatformRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >=0;
        }

        public async Task<IEnumerable<Platform>> GetAllAsync()
        {
            return await _context.Platforms.ToListAsync();
        }

        public async Task<Platform> GetPlatformByIdAsync(int Id)
        {
            return await _context.Platforms.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task CreatePlatformAsync(Platform platform)
        {
            if (platform is null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            await _context.Platforms.AddAsync(platform);
        }
    }
}
