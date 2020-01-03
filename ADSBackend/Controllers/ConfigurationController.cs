using Scholarships.Data;
using Scholarships.Models.ConfigurationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.Configuration Configuration;

        public ConfigurationController(ApplicationDbContext context, Services.Configuration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            var viewModel = new ConfigurationViewModel
            {
                AttachmentFilePath = Configuration.Get("RSSFeedUrl"),
                PrivacyPolicyUrl = Configuration.Get("PrivacyPolicyUrl")
            };

            return View(viewModel);
        }

        // POST: Configuration/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConfigurationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Configuration.Set("AttachmentFilePath", viewModel.AttachmentFilePath);
                Configuration.Set("PrivacyPolicyUrl", viewModel.PrivacyPolicyUrl);

                await Configuration.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View("Index", viewModel);
        }
    }
}