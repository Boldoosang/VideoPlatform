using System.ComponentModel.DataAnnotations;

namespace VideoPlatform.Web.Models
{
    public class FileUploadModel
    {
        [Required(ErrorMessage = "Please select a video file.")]
        public IFormFile? File { get; set; }
    }
}
