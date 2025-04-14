using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Domain.DTOs {
    public class PublishedVideoListingDTO {
        public IEnumerable<Season>? Seasons { get; set; }
        public IEnumerable<Episode>? StandaloneEpisodes { get; set; }
    }
}
