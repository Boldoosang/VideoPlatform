using Microsoft.AspNetCore.Authorization;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.Models;
using VideoPlatform.Web.Models;

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

        public IActionResult UploadEdited()
        {
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
        public async Task<IActionResult> UploadVideo(FileUploadModel fileUploadModel)
        {
            if (!ModelState.IsValid)
                return View(fileUploadModel);

            var file = fileUploadModel.File;

            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "No file was selected!";
                return RedirectToAction("Upload");
            }

            const long maxFileSize = 1L * 1024 * 1024 * 1024; // 1GB in bytes

            if (file.Length > maxFileSize)
                return BadRequest("File size exceeds the 1GB limit.");

            var allowedMimeTypes = new[]
            {
                "video/mp4",
                "video/webm",
            };

            var allowedExtensions = new[]
            {
                ".mp4", ".webm"
            };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Only video files are allowed (invalid extension).");

            if (!allowedMimeTypes.Contains(file.ContentType))
                return BadRequest("Only video files are allowed (invalid content type).");

            var container = _blobServiceClient.GetBlobContainerClient("videos");
            var blob = container.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, overwrite: true);
            }

            TempData["Success"] = "Video uploaded successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PublishEpisode(int episodeId) {
            var episode = await _episodeRepository.GetEpisodeAsync(episodeId);
            if (episode == null) return NotFound();

            episode.IsPublished = true;
            await _episodeRepository.UpdateEpisodeAsync(episode);

            TempData["Success"] = "Episode updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadEditedVideo(IFormFile file) {
            if (file == null || file.Length == 0)
                return BadRequest("Please select a file.");

            var allowedMimeTypes = new[]
            {
                "video/mp4",
                "video/webm",
            };

            var allowedExtensions = new[]
            {
                ".mp4", ".webm"
            };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Only video files are allowed (invalid extension).");

            if (!allowedMimeTypes.Contains(file.ContentType))
                return BadRequest("Only video files are allowed (invalid content type).");

            var container = _blobServiceClient.GetBlobContainerClient("editedvideos");
            var blob = container.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream()) {
                await blob.UploadAsync(stream, true);
                TempData["Success"] = "Video uploaded successfully!";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVideo(string data) {
            try {
                var container = _blobServiceClient.GetBlobContainerClient("videos");

                var blob = container.GetBlobClient(data);

                bool videoInUse = await _episodeRepository.IsVideoInUseAsync(data);

                if (videoInUse)
                {
                    TempData["Error"] = "Video is currently in use by an episode!";
                    return RedirectToAction("Index");
                }

                if (await blob.ExistsAsync())
                {
                    var result = await blob.DeleteIfExistsAsync();
                    TempData["Success"] = "Video deleted successfully!";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                TempData["Error"] = "Unable to delete video.";
                return View("Error", new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePublishedVideo(string data)
        {
            try
            {
                var container = _blobServiceClient.GetBlobContainerClient("publishedvideos");

                var blob = container.GetBlobClient(data);

                bool videoInUse = await _episodeRepository.IsVideoInUseAsync(data);

                if (videoInUse)
                {
                    TempData["Error"] = "Video is currently in use by an episode!";
                    return RedirectToAction("Index");
                }

                if (await blob.ExistsAsync())
                {
                    var result = await blob.DeleteIfExistsAsync();
                    TempData["Success"] = "Video deleted successfully!";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to delete video.";
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

                bool videoInUse = await _episodeRepository.IsVideoInUseAsync(data);

                if (videoInUse)
                {
                    TempData["Error"] = "Video is currently in use by an episode!";
                    return RedirectToAction("Index");
                }

                if (await blob.ExistsAsync())
                {
                    var result = await blob.DeleteIfExistsAsync();
                    TempData["Success"] = "Video deleted successfully!";
                } 

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to delete video. Please ensure it is not used by an episode.";
                return View("Error", new { message = ex.Message });
            }
        }
    }
}
