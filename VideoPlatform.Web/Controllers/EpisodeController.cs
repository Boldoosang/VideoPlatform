using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Web.Controllers {
    [Authorize(Roles = "Admin")]
    public class EpisodeController : Controller {
        private readonly IEpisodeRepository _episodeRepository;
        private readonly ISeasonRepository _seasonRepository;
        public EpisodeController(IEpisodeRepository episodeRepository, ISeasonRepository seasonRepository) {
            _episodeRepository = episodeRepository;
            _seasonRepository = seasonRepository;
        }

        public IActionResult Index() {
            var episodes = _episodeRepository.GetAllEpisodesAsync();
            return View(episodes);
        }

        [HttpGet]
        public IActionResult Create() {
            ViewBag.Seasons = _seasonRepository.GetAllSeasonsAsync(); // For dropdown selection
            return View();
        }

        [HttpPost]
        public IActionResult Create(Episode model) {
            if (ModelState.IsValid) {
                _episodeRepository.AddEpisodeAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
