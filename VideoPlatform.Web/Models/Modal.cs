using Microsoft.AspNetCore.Connections;

namespace VideoPlatform.Web.Models {
    public class Modal {
        public required string Id { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }
        public required SubmissionButton SubmissionButton { get; set; }
    }
}
