using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlatform.Domain.Models {
    public class Video {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string FilePath { get; set; }
        public required DateTime UploadDate { get; set; }
        public required int Duration { get; set; }
    }
}
