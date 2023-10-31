using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebShopMvcUI.Models;

namespace WebShopMvcUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

        public async Task<IActionResult> Index(string sterm = "", int genreId = 0)
        {
            IEnumerable<Item> items = await _homeRepository.GetItems(sterm, genreId);
            IEnumerable<Genre> genres = await _homeRepository.Genres();
            ItemDisplayModel itemModel = new ItemDisplayModel
            {
                Items = items,
                Genres = genres,
                STerm = sterm,
                GenreId = genreId
            };

            return View(itemModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}