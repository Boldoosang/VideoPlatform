using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VideoPlatform.Domain.DTOs;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.Models;
using VideoPlatform.Infrastructure;

namespace VideoPlatform.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EpisodeController : Controller
    {
        private readonly IEpisodeRepository _episodeRepository;
        private readonly ISeasonRepository _seasonRepository;
        private readonly BlobServiceClient _blobServiceClient;

        public EpisodeController(IEpisodeRepository episodeRepository, ISeasonRepository seasonRepository, BlobServiceClient blobServiceClient)
        {
            _episodeRepository = episodeRepository;
            _seasonRepository = seasonRepository;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Episode
        public async Task<IActionResult> Index()
        {
            var episodes = await _episodeRepository.GetAllEpisodesAsync();
            return View(episodes);
        }

        // GET: Episode/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Watch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _episodeRepository.GetEpisodeAsync(id.Value);

            if (episode == null)
            {
                return NotFound();
            }


            if (!episode.IsPublished)
            {
                if (User.IsInRole("Admin"))
                {
                    ViewBag.ViewingAsAdmin = true;
                } else
                {
                    return NotFound();
                }
            }

            EpisodePlaybackDTO episodePlaybackDTO = new EpisodePlaybackDTO
            {
                CurrentEpisode = episode,
                RelatedEpisodes = (await _episodeRepository.GetAllEpisodesAsync()).Where(e => e.IsPublished)
                    .ToList()
            };
            return View(episodePlaybackDTO);
        }

        // GET: Episode/Create
        public async Task <IActionResult> Create()
        {
            var editedVideoList = await GetEditedVideoList("editedvideos");
            var seasons = await _seasonRepository.GetAllSeasonsAsync(); 
            ViewData["SeasonId"] = new SelectList(seasons, "Id", "Title");
            ViewData["editedVideoList"] = new SelectList(editedVideoList, "FilePath", "Title");
            return View();
        }

        public async Task<List<Video>> GetEditedVideoList(string containerName) {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = container.GetBlobsAsync();

            var editedVideoList = new List<Video>();

            await foreach (var blob in blobs) {
                var blobClient = container.GetBlobClient(blob.Name);
                var blobProperties = container.GetBlobClient(blob.Name).GetProperties();

                editedVideoList.Add(new Video() {
                    Title = blob.Name,
                    FilePath = blobClient.Uri.ToString(),
                    UploadDate = blobProperties.Value.LastModified.DateTime,
                });
            }

            return editedVideoList;
        }

        // POST: Episode/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,PublishDate,FilePath,IsPublished,SeasonId")] Episode episode)
        {
            var editedVideoList = await GetEditedVideoList("editedvideos");

            if (ModelState.IsValid)
            {
                await _episodeRepository.AddEpisodeAsync(episode);
                TempData["Success"] = "Episode created successfully!";
                return RedirectToAction(nameof(Index));
            }
            var seasons = await _seasonRepository.GetAllSeasonsAsync();
            ViewData["SeasonId"] = new SelectList(seasons, "Id", "Title");
            ViewData["editedVideoList"] = new SelectList(editedVideoList, "FilePath", "Title");
            return View(episode);
        }

        // GET: Episode/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _episodeRepository.GetEpisodeAsync(id.Value);
            if (episode == null)
            {
                return NotFound();
            }
            var seasons = await _seasonRepository.GetAllSeasonsAsync();
            ViewData["SeasonId"] = new SelectList(seasons, "Id", "Title", episode.SeasonId);
            var editedVideoList = await GetEditedVideoList("editedvideos");
            ViewData["editedVideoList"] = new SelectList(editedVideoList, "FilePath", "Title");
            return View(episode);
        }

        // POST: Episode/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,PublishDate,FilePath,IsPublished,SeasonId")] Episode episode)
        {
            if (id != episode.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _episodeRepository.UpdateEpisodeAsync(episode);
                    TempData["Success"] = "Episode updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EpisodeExists(episode.Id))
                    {
                        TempData["Error"] = "Episode does not exist!";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var seasons = await _seasonRepository.GetAllSeasonsAsync();
            ViewData["SeasonId"] = new SelectList(seasons, "Id", "Title", episode.SeasonId);
            var editedVideoList = await GetEditedVideoList("editedvideos");
            ViewData["editedVideoList"] = new SelectList(editedVideoList, "FilePath", "Title");
            return View(episode);
        }

        // GET: Episode/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _episodeRepository.GetEpisodeAsync(id.Value);
            if (episode == null)
            {
                return NotFound();
            }

            return View(episode);
        }

        // POST: Episode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var episode = await _episodeRepository.GetEpisodeAsync(id);
            if (episode != null)
            {
                await _episodeRepository.DeleteEpisodeByIdAsync(episode.Id);
                TempData["Success"] = "Episode deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EpisodeExists(int id)
        {
            return await _episodeRepository.EpisodeExists(id);
        }

        [HttpGet("api/episodes")]
        [Authorize]
        public async Task<IActionResult> GetAllEpisodes()
        {
            var episodes = await _episodeRepository.GetAllEpisodesAsync();

            var episodeDTOs = episodes.Select(e => new EpisodeDTO
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                PublishDate = e.PublishDate,
                FilePath = e.FilePath,
                IsPublished = e.IsPublished,
                SeasonTitle = e.Season?.Title ?? "Standalone Episode"
            });

            return new JsonResult(new { data = episodeDTOs });
        }
    }
}
