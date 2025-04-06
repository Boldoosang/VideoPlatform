using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Infrastructure.Repositories {
    public class SeasonRepository : ISeasonRepository {

        private readonly VideoPlatformContext _context;   
        public SeasonRepository(VideoPlatformContext context) {
            _context = context;
        }
        public async Task AddSeasonAsync(Season season) {
            await _context.Seasons.AddAsync(season);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task DeleteSeasonByIdAsync(int SeasonId) {
            var season = await _context.Seasons.FindAsync(SeasonId);
            if (season != null) {
                _context.Seasons.Remove(season);
                await _context.SaveChangesAsync();
            }
            return;
        }

        public async Task<IEnumerable<Season>> GetAllSeasonsAsync() {
            return await _context.Seasons.ToListAsync();
        }

        public async Task<Season?> GetSeasonAsync(int SeasonId) {
            return await _context.Seasons.FirstOrDefaultAsync(v => v.Id == SeasonId);
        }

        public async Task<bool> SeasonExists(int SeasonId) {
            return await _context.Seasons.AnyAsync(e => e.Id == SeasonId);
        }
        

        public async Task UpdateSeasonAsync(Season season) {
            _context.Seasons.Update(season);
            await _context.SaveChangesAsync();
        }
    }
}
