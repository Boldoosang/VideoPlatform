using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VideoPlatform.Domain.DTOs;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.Models;
using VideoPlatform.Infrastructure;
using VideoPlatform.Infrastructure.Repositories;

namespace VideoPlatform.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SeasonController : Controller
    {
        private readonly ISeasonRepository _seasonRepository;

        public SeasonController(ISeasonRepository seasonRepository)
        {
            _seasonRepository = seasonRepository;
        }

        // GET: Seasons
        public async Task<IActionResult> Index()
        {
            return View(await _seasonRepository.GetAllSeasonsAndEpisodesAsync());
        }

        // GET: Seasons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _seasonRepository.GetSeasonAsync(id.Value);
 
            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }

        // GET: Seasons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Seasons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SeasonNumber,Title,Description,ReleaseDate,IsPublished")] Season season)
        {
            if (ModelState.IsValid)
            {
                await _seasonRepository.AddSeasonAsync(season);
                TempData["Success"] = "Season created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(season);
        }

        // GET: Seasons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _seasonRepository.GetSeasonAsync(id.Value);
            if (season == null)
            {
                return NotFound();
            }
            return View(season);
        }

        // POST: Seasons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SeasonNumber,Title,Description,ReleaseDate,IsPublished")] Season season)
        {
            if (id != season.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    try
                    {
                        await _seasonRepository.UpdateSeasonAsync(season);
                        TempData["Success"] = "Season updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException e)
                    {
                        ModelState.AddModelError("SeasonNumber", "This season number is already taken.");
                        return View(season);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await SeasonExists(season.Id))
                    {
                        TempData["Error"] = "Season does not exist!";
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(season);
        }

        // GET: Seasons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _seasonRepository.GetSeasonAndEpisodesAsync(id.Value);

            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }

        // POST: Seasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var season = await _seasonRepository.GetSeasonAsync(id);
            if (season != null)
            {
                await _seasonRepository.DeleteSeasonByIdAsync(season.Id);
            }
            TempData["Success"] = "Season deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SeasonExists(int id)
        {
            return await _seasonRepository.SeasonExists(id);
        }

        [HttpGet("api/seasons")]
        [Authorize]
        public async Task<IActionResult> GetAllSeasons()
        {
            var seasons = await _seasonRepository.GetAllSeasonsAndEpisodesAsync();

            var seasonDTO = seasons.Select(s => new SeasonDTO
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                ReleaseDate = s.ReleaseDate,
                SeasonNumber = s.SeasonNumber,
                EpisodeCount = s.Episodes?.Count() ?? 0,
                IsPublished = s.IsPublished
            });

            return new JsonResult(new { data = seasonDTO });
        }
    }
}
