using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Infrastructure.Repositories {
    public class VideoRepository : IVideoRepository {

        private readonly VideoPlatformContext _context;   
        public VideoRepository(VideoPlatformContext context) {
            _context = context;
        }
        public async Task AddVideoAsync(Video video) {
            await _context.Videos.AddAsync(video);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task DeleteVideoByIdAsync(int videoId) {
            var video = await _context.Videos.FindAsync(videoId);
            if (video != null) {
                _context.Videos.Remove(video);
                await _context.SaveChangesAsync();
            }
            return;
        }

        public async Task<IEnumerable<Video>> GetAllVideosAsync() {
            return await _context.Videos.ToListAsync();
        }

        public async Task<Video?> GetVideoAsync(int videoId) {
            return await _context.Videos.FirstOrDefaultAsync(v => v.Id == videoId);
        }

        public async Task UpdateVideoAsync(Video video) {
            _context.Videos.Update(video);
            await _context.SaveChangesAsync();
        }
    }
}
