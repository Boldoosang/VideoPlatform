using Microsoft.AspNetCore.Authorization;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Web.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController(BlobServiceClient blobServiceClient, IEpisodeRepository episodeRepository) : Controller {

        private readonly BlobServiceClient _blobServiceClient = blobServiceClient;
        private readonly IEpisodeRepository _episodeRepository = episodeRepository;

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

            var videoList = new List<Video>();

            await foreach (var blob in blobs) {
                var blobClient = container.GetBlobClient(blob.Name);
                var blobProperties = container.GetBlobClient(blob.Name).GetProperties();

                videoList.Add(new Video() { 
                    Title = blob.Name,
                    FilePath = blobClient.Uri.ToString(),
                    UploadDate = blobProperties.Value.LastModified.DateTime,            
                });
            }

            return View(videoList);
        }

        [HttpGet]
        public async Task<IActionResult> EditedVideoLibrary() {
            var container = _blobServiceClient.GetBlobContainerClient("editedvideos");
            var blobs = container.GetBlobsAsync();

            var videoList = new List<Video>();

            await foreach (var blob in blobs) {
                var blobClient = container.GetBlobClient(blob.Name);
                var blobProperties = container.GetBlobClient(blob.Name).GetProperties();

                videoList.Add(new Video() {
                    Title = blob.Name,
                    FilePath = blobClient.Uri.ToString(),
                    UploadDate = blobProperties.Value.LastModified.DateTime,
                });
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
        public async Task<IActionResult> UploadEditedVideo(IFormFile file) {
            var container = _blobServiceClient.GetBlobContainerClient("editedvideos");
            var blob = container.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream()) {
                await blob.UploadAsync(stream, true);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVideo(string data) {
            try {
                var container = _blobServiceClient.GetBlobContainerClient("videos");

                var blob = container.GetBlobClient(data);

                if (await blob.ExistsAsync()) {
                    await blob.DeleteIfExistsAsync();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                return View("Error", new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEditedVideo(string data)
        {
            try
            {
                var container = _blobServiceClient.GetBlobContainerClient("editedvideos");

                var blob = container.GetBlobClient(data);

                if (await blob.ExistsAsync())
                {
                    await blob.DeleteIfExistsAsync();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePublishedVideos(string data)
        {
            try
            {
                var container = _blobServiceClient.GetBlobContainerClient("publishedvideos");

                var blob = container.GetBlobClient(data);

                if (await blob.ExistsAsync())
                {
                    await blob.DeleteIfExistsAsync();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new { message = ex.Message });
            }
        }
    }
}
