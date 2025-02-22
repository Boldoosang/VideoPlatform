namespace VideoPlatform.Domain.Models {
    public class Episode {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime PublishDate { get; set; }
    }
}