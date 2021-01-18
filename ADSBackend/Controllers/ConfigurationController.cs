using System;
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
        private readonly Services.Configuration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ConfigurationController(ApplicationDbContext context, Services.Configuration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index(int? error)
        {
            var viewModel = new ConfigurationViewModel
            {
                ErrorMessage = error == null ? "" : "Site configuration is incomplete",
                ApplicationEmail = _configuration.Get("ApplicationEmail"),
                ApplicationEmailPassword = _configuration.Get("ApplicationEmailPassword"),
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
                _configuration.Set("ApplicationEmail", viewModel.ApplicationEmail);

                if (!String.IsNullOrEmpty(viewModel.ApplicationEmailPassword))
                    _configuration.Set("ApplicationEmailPassword", viewModel.ApplicationEmailPassword);

                await _configuration.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View("Index", viewModel);
        }
    }
}