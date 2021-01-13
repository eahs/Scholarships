using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scholarships.Data;
using Scholarships.Models.HomeViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Admin");
            }

            var articles = await _context.Article.Where(art => art.Published)
                                                 .Take(4)
                                                 .OrderByDescending(art => art.PublishDate)
                                                 .ToListAsync();

            HomeViewModel vm = new HomeViewModel
            {
                Articles = articles
            };

            return View(vm);
        }

        public async Task<IActionResult> News (int? id)
        {
            if (id == null)
                return NotFound();

            var article = await _context.Article.Where(art => art.Published).FirstOrDefaultAsync(art => art.ArticleId == id);

            if (article == null)
                return NotFound();

            return View(article);
        }

        public IActionResult Resources()
        {
            return View();
        }

    }
}
