﻿using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace VideoPlatform.Domain.Models {
    public class Episode {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public required string FilePath { get; set; }
        public string? ThumbnailFilePath { get; set; }
        public bool IsPublished { get; set; }
        public int? SeasonId { get; set; }
        public virtual Season? Season { get; set; }
    }
}