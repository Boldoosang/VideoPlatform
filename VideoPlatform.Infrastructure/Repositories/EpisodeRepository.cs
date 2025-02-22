using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Infrastructure.Repositories {
    public class EpisodeRepository : IEpisodeRepository {

        private readonly VideoPlatformContext _context;   
        public EpisodeRepository(VideoPlatformContext context) {
            _context = context;
        }
        public async Task AddEpisodeAsync(Episode episode) {
            await _context.Episodes.AddAsync(episode);
            return;
        }

        public async Task DeleteEpisodeByIdAsync(int episodeId) {
            var episode = await _context.Episodes.FindAsync(episodeId);
            if (episode != null) {
                _context.Episodes.Remove(episode);
                await _context.SaveChangesAsync();
            }
            return;
        }

        public async Task<IEnumerable<Episode>> GetAllEpisodesAsync() {
            return await _context.Episodes.ToListAsync();
        }

        public async Task<Episode?> GetEpisodeAsync(int episodeId) {
            return await _context.Episodes.FirstOrDefaultAsync(v => v.Id == episodeId);
        }

        public async Task UpdateEpisodeAsync(Episode episode) {
            _context.Episodes.Update(episode);
            await _context.SaveChangesAsync();
        }
    }
}
