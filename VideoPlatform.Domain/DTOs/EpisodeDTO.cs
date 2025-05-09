﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlatform.Domain.DTOs
{
    public class EpisodeDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public required string FilePath { get; set; }
        public bool IsPublished { get; set; }
        public required string SeasonTitle { get; set; }
    }
}
