using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VideoPlatform.Web.Controllers {
    [Authorize(Roles = "Admin")]
    public class VideoEditorController : Controller {

        private readonly BlobServiceClient _blobServiceClient;
        public VideoEditorController(BlobServiceClient blobServiceClient) {
            _blobServiceClient = blobServiceClient;
        }

        public IActionResult Edit(string videoUrl) {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/html");
        }
    }
}
