namespace VideoPlatform.Web.Models {
    public class Button {
        public required string Text { get; set; }
        public string? CssClass { get; set; }
        public required string Action { get; set; }
        public string? Controller { get; set; }
    }

    public class SubmissionButton : Button {
        public required string Method { get; set; }
    }
}
