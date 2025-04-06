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
            var episodes = await _episodeRepository.GetAllEpisodesAsync();
            var publishedEpisodes = episodes
                .Where(e => e.IsPublished)
                .OrderByDescending(e => e.PublishDate)
                .ToList();

            var seasons = await _seasonRepository.GetAllSeasonsAsync();
            var standaloneEpisodes = await _episodeRepository.GetStandaloneEpisodesAsync();

            var publishedVideoListingDTO = new PublishedVideoListingDTO {
                Seasons = seasons.ToList(),
                StandaloneEpisodes = standaloneEpisodes.Where(e => e.IsPublished)
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
