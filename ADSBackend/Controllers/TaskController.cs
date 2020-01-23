using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileTypeChecker;
using FileTypeChecker.Abstracts;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scholarships.Data;
using Scholarships.Services;
using Scholarships.Tasks;
using Serilog;

namespace Scholarships.Controllers
{
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;
        private readonly Services.Configuration Configuration;

        public TaskController(ApplicationDbContext context, DataService dataService, Services.Configuration configurationService)
        {
            _context = context;
            _dataService = dataService;
            Configuration = configurationService;
        }

        public ActionResult Transcripts()
        {
            return View();
        }

        public async Task<ActionResult> UploadTranscripts(IFormFile file)
        {
            string transcriptPath = Configuration.ConfigPath.TranscriptsPath;

            if (string.IsNullOrEmpty(transcriptPath))
                return RedirectToAction("Index", "Configuration", new { error = 1 });

            // Create the directory if it doesn't exist
            System.IO.Directory.CreateDirectory(transcriptPath);

            if (file.ContentType == "application/pdf" && file.Length > 0)
            {
                using (var ftStream = file.OpenReadStream())
                {
                    // Examine the file byte structure to validate the type
                    IFileType fileType = FileTypeValidator.GetFileType(ftStream);

                    string filePath = System.IO.Path.Combine(transcriptPath, "transcripts.pdf");

                    if (fileType.Extension == "pdf")
                    {
                        try
                        {
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Fire off task to process the pdf
                            BackgroundJob.Enqueue<IGenerateTranscripts>(
                                generator => generator.Execute());

                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Unable to save uploaded transcripts.pdf file");
                        }
                    }
                    {
                        Log.Error("transcripts.pdf is not a pdf file");
                    }
                }
            }

            return View();
        }

        public IActionResult Hangfire ()
        {
            return View();
        }
    }
}