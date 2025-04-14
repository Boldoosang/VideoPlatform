using Microsoft.AspNetCore.Authorization;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.Models;
using VideoPlatform.Web.Models;
using VideoPlatform.Infrastructure.Repositories;

namespace VideoPlatform.Web.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {

        private readonly IVideoStorageAccessor _videoStorageAccessor;
        private readonly IEpisodeRepository _episodeRepository;

        public AdminController(IEpisodeRepository episodeRepository, IVideoStorageAccessor videoStorageAccessor)
        {
            _videoStorageAccessor = videoStorageAccessor;
            _episodeRepository = episodeRepository;
        }

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
            var videoList = await _videoStorageAccessor.GetContainerVideoListAsync("videos");

            return View(videoList);
        }

        [HttpGet]
        public async Task<IActionResult> EditedVideoLibrary() {
            var videoList = await _videoStorageAccessor.GetContainerVideoListAsync("editedvideos");

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

            //if (!allowedMimeTypes.Contains(file.ContentType))
            //    return BadRequest("Only video files are allowed (invalid content type).");

            var uploaded = await _videoStorageAccessor.UploadVideoAsync("videos", file.FileName, file.OpenReadStream());

            if(!uploaded)
            {
                TempData["Error"] = "Unable to upload video. Please ensure a matching file name does not already exists.";
                return RedirectToAction("Upload");
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

            //if (!allowedMimeTypes.Contains(file.ContentType))
            //    return BadRequest("Only video files are allowed (invalid content type).");

            var uploaded = await _videoStorageAccessor.UploadVideoAsync("editedvideos", file.FileName, file.OpenReadStream());

            if (!uploaded)
            {
                TempData["Error"] = "Unable to upload video. Please ensure a matching file name does not already exists.";
                return RedirectToAction("Upload");
            }

            TempData["Success"] = "Video uploaded successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVideo(string data) {
            try {
                bool videoInUse = await _episodeRepository.IsVideoInUseAsync(data);

                if (videoInUse)
                {
                    TempData["Error"] = "Video is currently in use by an episode!";
                    return RedirectToAction("Index");
                }

                bool deleted = await _videoStorageAccessor.DeleteVideoIfExistsAsync("videos", data);
                if (deleted)
                {
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

                bool videoInUse = await _episodeRepository.IsVideoInUseAsync(data);

                if (videoInUse)
                {
                    TempData["Error"] = "Video is currently in use by an episode!";
                    return RedirectToAction("Index");
                }

                bool deleted = await _videoStorageAccessor.DeleteVideoIfExistsAsync("publishedvideos", data);
                if (deleted)
                {
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
                bool videoInUse = await _episodeRepository.IsVideoInUseAsync(data);

                if (videoInUse)
                {
                    TempData["Error"] = "Video is currently in use by an episode!";
                    return RedirectToAction("Index");
                }

                bool deleted = await _videoStorageAccessor.DeleteVideoIfExistsAsync("editedvideos", data);
                if (deleted)
                {
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
