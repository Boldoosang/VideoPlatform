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
            if (!string.IsNullOrWhiteSpace(episode.FilePath)) {
                episode.ThumbnailFilePath = Path.ChangeExtension(episode.FilePath, ".png");
            }
            await _context.Episodes.AddAsync(episode);
            await _context.SaveChangesAsync();
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
            return await _context.Episodes.Include(e => e.Season).OrderByDescending(e => e.PublishDate).ToListAsync();
        }

        public async Task<Episode?> GetEpisodeAsync(int episodeId) {
            return await _context.Episodes.Include(e => e.Season).FirstOrDefaultAsync(v => v.Id == episodeId);
        }

        public async Task<IEnumerable<Episode>> GetStandaloneEpisodesAsync() {
            return await _context.Episodes.Where(e => e.SeasonId == null).ToListAsync();
        }

        public async Task<bool> EpisodeExists(int episodeId) {
            return await _context.Episodes.AnyAsync(e => e.Id == episodeId);
        }

        public async Task UpdateEpisodeAsync(Episode episode) {
            if (!string.IsNullOrWhiteSpace(episode.FilePath)) {
                var thumbnailPath = Path.ChangeExtension(episode.FilePath, ".png");
                episode.ThumbnailFilePath = File.Exists(thumbnailPath) ? thumbnailPath : null;
            }
            _context.Episodes.Update(episode);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsVideoInUseAsync(string videoName)
        {
            return await _context.Episodes.AnyAsync(e => e.FilePath.EndsWith(videoName));
        }
    }
}
