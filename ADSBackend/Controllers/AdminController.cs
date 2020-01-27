using System.Linq;
using Scholarships.Models.HomeViewModels;
using Scholarships.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scholarships.Data;
using Scholarships.Models;
using Scholarships.Services;
using Serilog;

namespace Scholarships.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, DataService dataService)
        {
            _userManager = userManager;
            _context = context;
            _dataService = dataService;
        }

        public async Task<IActionResult> Index()
        {
            var profile = await _dataService.GetProfileAsync();

            if (profile == null)
            {
                Log.Error("User profile was found to be null on AdminController->Index");
                return NotFound();
            }

            var articles = await _context.Article.Where(art => art.Published)
                .Take(4)
                .OrderByDescending(art => art.PublishDate)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                User = await _userManager.GetUserAsync(User),
                Articles = articles,
                ScholarshipUpdates = await _dataService.GetScholarshipUpdates(5),
                ScholarshipsUpcomingDeadlines = await _dataService.GetScholarshipDeadlines(10),
                MyApplications = await _dataService.GetMyApplications(profile.ProfileId),
                MyFavorites = await _dataService.GetMyFavorites(profile.ProfileId),
                CompletedProfile = profile.FormCompletedBasic && 
                                   profile.FormCompletedAcademicPerformance &&
                                   profile.FormCompletedCollegePlans &&
                                   profile.FormCompletedExtraCurricular,
                CompletedScholarshipApplication = await _context.Application.FirstOrDefaultAsync(app => app.Submitted && app.ProfileId == profile.ProfileId) != null
            };

            return View(viewModel);
        }
    }
}
