using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Domain.Interfaces {
    public interface IVideoRepository {
        Task<Video?> GetVideoAsync(int videoId);
        Task<IEnumerable<Video>> GetAllVideosAsync();
        Task AddVideoAsync(Video video);
        Task UpdateVideoAsync(Video video);
        Task DeleteVideoByIdAsync(int videoId);
    }
}
