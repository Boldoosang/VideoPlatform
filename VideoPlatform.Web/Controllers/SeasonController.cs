using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Web.Controllers {
    [Authorize(Roles = "Admin")]
    public class SeasonController : Controller {


        private readonly ISeasonRepository _seasonRepository;

        public SeasonController(ISeasonRepository seasonRepository) {
            _seasonRepository = seasonRepository;
        }

        public IActionResult Index() {
            var seasons = _seasonRepository.GetAllSeasonsAsync();
            return View(seasons);
        }

        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Season model) {
            if (ModelState.IsValid) {
                _seasonRepository.AddSeasonAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
