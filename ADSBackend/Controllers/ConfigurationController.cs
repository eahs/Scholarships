using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Scholarships.Data;
using Scholarships.Models.ConfigurationViewModels;
using System.Threading.Tasks;

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
                StudentDataFilePath = Configuration.Get("StudentDataFilePath"),
                ApplicationEmail = Configuration.Get("ApplicationEmail"),
                ApplicationEmailPassword = Configuration.Get("ApplicationEmailPassword"),
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
                Configuration.Set("StudentDataFilePath", viewModel.StudentDataFilePath);
                Configuration.Set("ApplicationEmail", viewModel.ApplicationEmail);

                if (viewModel.ApplicationEmailPassword != null)
                    Configuration.Set("ApplicationEmailPassword", viewModel.ApplicationEmailPassword);

                await Configuration.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View("Index", viewModel);
        }
    }
}