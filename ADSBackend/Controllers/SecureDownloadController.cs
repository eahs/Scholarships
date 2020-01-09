using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Scholarships.Data;
using Scholarships.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Serilog;

namespace Scholarships.Controllers
{
    public class SecureDownloadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.Configuration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly DataService _dataService;

        public SecureDownloadController(ApplicationDbContext context, DataService dataService, Services.Configuration configurationService, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _dataService = dataService;
            Configuration = configurationService;
            _hostingEnvironment = hostingEnvironment;
        }

        // How to create full url's for a particular file attachment in a controller:
        // string url = Url.Link("securedownload", new { id, filename });
        public async Task<IActionResult> Download(int id, string filename)
        {
            var profile = await _dataService.GetProfileAsync();

            if (profile == null)
                return NotFound();

            var fa = await _context.FileAttachment.Include(a => a.FileAttachmentGroup)
                                                  .FirstOrDefaultAsync(a => a.FileAttachmentId == id);

            // Does this file exist?
            if (fa == null || fa.FileName != filename)
            {
                Log.Information("User with profile Id {0} tried accessing unavailable file {1}", profile.ProfileId, filename);
                return NotFound();
            }

            // Does user have permission to download this file?  TODO: Allow admins to access files
            if (fa.FileAttachmentGroup.ProfileId != profile.ProfileId)
            {
                return NotFound();
            }

            var filePath = System.IO.Path.Combine(Configuration.Get("AttachmentFilePath"),
                                                  fa.FileSubPath,
                                                  fa.SecureFileName);

            try
            {
                var stream = new FileStream(filePath, FileMode.Open);
                return new FileStreamResult(stream, fa.ContentType);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error downloading file: {0}", filePath);
                return NotFound();
            }

        }
    }
}