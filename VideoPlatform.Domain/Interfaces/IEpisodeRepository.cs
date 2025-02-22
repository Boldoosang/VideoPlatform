using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Domain.Interfaces {
    public interface IEpisodeRepository {
        Task<Episode?> GetEpisodeAsync(int episodeId);
        Task<IEnumerable<Episode>> GetAllEpisodesAsync();
        Task AddEpisodeAsync(Episode episode);
        Task UpdateEpisodeAsync(Episode episode);
        Task DeleteEpisodeByIdAsync(int episodeId);
    }
}
