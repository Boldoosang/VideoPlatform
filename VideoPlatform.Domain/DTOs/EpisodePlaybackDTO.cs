using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Domain.DTOs
{
    public class EpisodePlaybackDTO
    {
        public required Episode CurrentEpisode { get; set; }
        public List<Episode> RelatedEpisodes { get; set; } = new();
    }
}
