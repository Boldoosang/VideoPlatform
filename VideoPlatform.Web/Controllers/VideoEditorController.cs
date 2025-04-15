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
        public IActionResult Edit(string videoSrc) {
            ViewBag.VideoUrl = videoSrc;
            return View();
        }

        public IActionResult Edit2(string videoSrc) {
            ViewBag.VideoUrl = videoSrc;
            return View();
        }
    }
}
