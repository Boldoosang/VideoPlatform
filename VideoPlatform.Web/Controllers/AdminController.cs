using Microsoft.AspNetCore.Authorization;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoPlatform.Domain.Interfaces;

namespace VideoPlatform.Web.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {

        private readonly BlobServiceClient _blobServiceClient;
        private readonly IEpisodeRepository _episodeRepository;

        public AdminController(BlobServiceClient blobServiceClient, IEpisodeRepository episodeRepository) {
            _blobServiceClient = blobServiceClient;
            _episodeRepository = episodeRepository;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Upload() {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VideoLibrary() {
            var container = _blobServiceClient.GetBlobContainerClient("videos");
            var blobs = container.GetBlobsAsync();

            var videoList = new List<string>();

            await foreach (var blob in blobs) {
                var blobUri = container.GetBlobClient(blob.Name).Uri.ToString();
                videoList.Add(blobUri);
            }

            return View(videoList);
        }

        [HttpPost]
        public async Task<IActionResult> UploadVideo(IFormFile file) {
            if (file == null || file.Length == 0)
                return BadRequest("Please select a file.");

            var container = _blobServiceClient.GetBlobContainerClient("videos");
            var blob = container.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream()) {
                await blob.UploadAsync(stream, true);
            }

            // Save metadata in the database (title, description, etc.)
            // do this later

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PublishEpisode(int episodeId) {
            var episode = await _episodeRepository.GetEpisodeAsync(episodeId);
            if (episode == null) return NotFound();

            episode.IsPublished = true;
            await _episodeRepository.UpdateEpisodeAsync(episode);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadToAzure(IFormFile file) {
            var container = _blobServiceClient.GetBlobContainerClient("videos");
            var blob = container.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream()) {
                await blob.UploadAsync(stream, true);
            }

            return Ok("Uploaded to Azure");
        }
    }
}
