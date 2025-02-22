using Microsoft.AspNetCore.Authorization;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace VideoPlatform.Web.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {

        private readonly BlobServiceClient _blobServiceClient;

        public AdminController(BlobServiceClient blobServiceClient) {
            _blobServiceClient = blobServiceClient;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Upload() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file) {
            if (file == null || file.Length == 0) {
                ViewBag.Message = "Please select a file.";
                return View();
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient("videos");
            var blobClient = containerClient.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream()) {
                await blobClient.UploadAsync(stream, true);
            }

            ViewBag.Message = "File uploaded successfully!";
            return View();
        }

    }
}
