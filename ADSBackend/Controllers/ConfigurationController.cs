using Scholarships.Data;
using Scholarships.Models.ConfigurationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Scholarships.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.Configuration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ConfigurationController(ApplicationDbContext context, Services.Configuration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index(int? error)
        {
            var viewModel = new ConfigurationViewModel
            {
                ErrorMessage = error == null ? "" : "Site configuration is incomplete",
                AttachmentFilePath = Configuration.Get("AttachmentFilePath"),
                TranscriptFilePath = Configuration.Get("TranscriptFilePath"),
                PrivacyPolicyUrl = Configuration.Get("PrivacyPolicyUrl"),
                RootWebPath = _hostingEnvironment.WebRootPath
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
                Configuration.Set("TranscriptFilePath", viewModel.TranscriptFilePath);
                Configuration.Set("PrivacyPolicyUrl", viewModel.PrivacyPolicyUrl);

                await Configuration.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View("Index", viewModel);
        }
    }
}