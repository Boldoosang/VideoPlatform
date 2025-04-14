using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlatform.Domain.Models {
    public class Season {
        public int Id { get; set; }
        public int SeasonNumber { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateTime ReleaseDate { get; set; }
        public required bool IsPublished { get; set; }
        public ICollection<Episode>? Episodes { get; set; } // Do the initialization thingy; consult Anand
    }
}
