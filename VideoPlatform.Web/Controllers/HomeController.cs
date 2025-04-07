using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.DTOs;
using VideoPlatform.Web.Models;

namespace VideoPlatform.Web.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly ISeasonRepository _seasonRepository;

        public HomeController(ILogger<HomeController> logger, IEpisodeRepository episodeRepository, ISeasonRepository seasonRepository) {
            _logger = logger;
            _episodeRepository = episodeRepository;
            _seasonRepository = seasonRepository;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index() {
            var seasons = (await _seasonRepository.GetAllSeasonsAndEpisodesAsync()).Where(s => s.IsPublished);
            var standaloneEpisodes = (await _episodeRepository.GetStandaloneEpisodesAsync()).Where(e => e.IsPublished);

            var publishedVideoListingDTO = new PublishedVideoListingDTO
            {
                Seasons = seasons.ToList(),
                StandaloneEpisodes = standaloneEpisodes.ToList()
            };

            return View(publishedVideoListingDTO);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
