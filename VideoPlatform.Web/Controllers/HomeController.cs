using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Web.Models;

namespace VideoPlatform.Web.Controllers {
    [Authorize]
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IEpisodeRepository _episodeRepository;

        public HomeController(ILogger<HomeController> logger, IEpisodeRepository episodeRepository) {
            _logger = logger;
            _episodeRepository = episodeRepository;
        }

        public async Task<IActionResult> Index() {
            var episodes = await _episodeRepository.GetAllEpisodesAsync();
            var publishedEpisodes = episodes
                .Where(e => e.IsPublished)
                .OrderByDescending(e => e.PublishDate)
                .ToList();

            return View(publishedEpisodes);
        }

        public IActionResult Privacy() {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
