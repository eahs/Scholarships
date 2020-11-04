using Scholarships.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPdf;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Scholarships.Models;
using Scholarships.Models.Forms;
using Scholarships.Models.ScholarshipViewModels;
using Scholarships.Services;
using Scholarships.Util;
using Serilog.Core;

namespace Scholarships.Tasks
{
    public class CreateApplicationPackage : ICreateApplicationPackage
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.Configuration Configuration;
        private readonly ViewRenderService _viewRenderService;
        private readonly DataService _dataService;

        public CreateApplicationPackage (ApplicationDbContext context, Services.Configuration configurationService, DataService dataService, ViewRenderService viewRenderService)
        {
            _context = context;
            _dataService = dataService;
            _viewRenderService = viewRenderService;
            Configuration = configurationService;
        }


        public void Execute()
        {
            try
            {
                AsyncHelpers.RunSync(CreatePackages);
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception thrown while executing ");
            }
        }

        public async Task CreatePackages()
        {
            string transcriptPath = Configuration.ConfigPath.TranscriptsPath;
            string joboutputPath = Configuration.ConfigPath.JobOutputPath;

            Log.Information("Beginning generation of application package");

            if (transcriptPath == null)
            {
                Log.Error("Transcripts path configuration variable has not been set by administrator");
                return;
            }

            if (joboutputPath == null)
            {
                Log.Error("Job output path configuration variable has not been set by administrator");
                return;
            }

            try
            {
                Directory.CreateDirectory(joboutputPath);
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to create directory to store job output");
                return;
            }

            var job = _context.Job.FirstOrDefault(j => !j.Completed && j.Type == "applications");

            if (job != null)
            {
                // Load in the profile picture mask
                PdfDocument pdfTranscriptPhotoOverlay = new PdfDocument("transcript_photo_mask.pdf");
                PdfDocument pdfTranscriptNameOverlay = new PdfDocument("transcript_name_mask.pdf");

                job.Completed = true;
                job.Started = DateTime.Now;
                job.StatusMessage = "Running";
                _context.Update(job);
                _context.SaveChanges();

                var scholarship = await _dataService.GetScholarship(job.ForeignKey);

                List<Application> applications = _context.Application.Include(ap => ap.Profile)
                                                                     .Where(s => s.ScholarshipId == job.ForeignKey && s.Submitted)
                                                                     .ToList();

                joboutputPath = Path.Combine(joboutputPath, "job" + job.JobId + ".pdf");

                //PdfDocument doc = new PdfDocument(joboutputPath);
                var Renderer = new IronPdf.HtmlToPdf();
                Renderer.PrintOptions.CustomCssUrl =
                    "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css";
                Renderer.PrintOptions.SetCustomPaperSizeInInches(8.5, 11);
                Renderer.PrintOptions.PrintHtmlBackgrounds = true;
                Renderer.PrintOptions.PaperOrientation = PdfPrintOptions.PdfPaperOrientation.Portrait;
                Renderer.PrintOptions.Title = "Scholarship Package";
                Renderer.PrintOptions.EnableJavaScript = true;
                Renderer.PrintOptions.RenderDelay = 50; //ms
                Renderer.PrintOptions.CssMediaType = PdfPrintOptions.PdfCssMediaType.Screen;
                Renderer.PrintOptions.DPI = 300;
                Renderer.PrintOptions.FitToPaperWidth = true;
                Renderer.PrintOptions.JpegQuality = 90;
                Renderer.PrintOptions.GrayScale = false;
                Renderer.PrintOptions.FitToPaperWidth = true;
                Renderer.PrintOptions.InputEncoding = Encoding.UTF8;
                Renderer.PrintOptions.Zoom = 100;
                Renderer.PrintOptions.CreatePdfFormsFromHtml = false;
                Renderer.PrintOptions.MarginTop = 40;  //millimeters
                Renderer.PrintOptions.MarginLeft = 20;  //millimeters
                Renderer.PrintOptions.MarginRight = 20;  //millimeters
                Renderer.PrintOptions.MarginBottom = 40;  //millimeters
                Renderer.PrintOptions.FirstPageNumber = 2; //use 2 if a coverpage  will be appended

                var coverpage = await _viewRenderService.RenderToStringAsync("_scholarshipcoverpagepartial", scholarship, "scholarships");

                PdfDocument doc = Renderer.RenderHtmlAsPdf(coverpage);

                foreach (var app in applications)
                {
                    QuestionSet qset = await _dataService.GetQuestionSetWithAnswers(app.AnswerGroupId, bypassProfileVerification: true);

                    ScholarshipApplyViewModel vm = new ScholarshipApplyViewModel
                    {
                        Application = app,
                        QuestionSet = qset,
                        Scholarship = scholarship
                    };

                    ApplicationPageViewModel applicationPage = new ApplicationPageViewModel();

                    // First render the core general profile
                    try
                    {
                        applicationPage.BasicProfile =
                            await _viewRenderService.RenderToStringAsync("_scholarshipapplicationpartial", vm,
                                "scholarships");
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Unable to render scholarship application partial when creating Application Package");
                    }

                    // Next render any additional questions that were provided
                    if (qset.Questions?.Count > 0)
                    {
                        // Render answers to questions
                        try
                        {
                            applicationPage.FormAnswers =
                                await _viewRenderService.RenderToStringAsync("questionsetappviewpartial", vm.QuestionSet,
                                    "answergroup");

                        }
                        catch (Exception e)
                        {
                            var serqset = JsonConvert.SerializeObject(vm.QuestionSet, Formatting.Indented, new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });

                            Log.Error(e, "Unable to render answers to questions for application #{0} of scholarship Id {1}", app.ApplicationId, scholarship.ScholarshipId);
                            Log.Information("Serialized Question Set: {0}", serqset);
                        }
                    }

                    string finalApplication =
                        await _viewRenderService.RenderToStringAsync("_scholarshipapplicationfull", applicationPage,
                            "scholarships");

                    PdfDocument page = Renderer.RenderHtmlAsPdf(finalApplication, "https://scholarships.eastonsd.org/");

                    doc.AppendPdf(page);

                    // Next render any file attachments
                    foreach (var answerset in vm.QuestionSet.AnswerSets)
                    {
                        foreach (var answer in answerset.Answers)
                        {
                            if (answer.FileAttachmentGroup == null) continue;

                            if (answer.FileAttachmentGroup.FileAttachments != null)
                            {
                                string lastFileTried = ""; // In case of error

                                try
                                {
                                    foreach (var file in answer.FileAttachmentGroup.FileAttachments)
                                    {
                                        var filePath = System.IO.Path.Combine(Configuration.ConfigPath.AttachmentPath,
                                            file.FileSubPath,
                                            file.SecureFileName);

                                        lastFileTried = filePath;

                                        if (file.ContentType == "application/pdf")
                                        {
                                            if (File.Exists(filePath))
                                            {
                                                PdfDocument pdfAttach = new PdfDocument(filePath);

                                                doc.AppendPdf(pdfAttach);
                                            }
                                        }
                                        else
                                        {
                                            if (File.Exists(filePath))
                                            {
                                                var subpath = Path.Combine(Configuration.ConfigPath.AttachmentPath,
                                                    file.FileSubPath);
                                                var fullpath = Path.GetFullPath(subpath);

                                                var html =
                                                    "<html style='width:100%;height:100%'><body style='width:100%;height:100%'><img style='max-width: 100%; max-height: 100vh; height: auto;' src='" +
                                                    file.SecureFileName + "'></body></html>";
                                                var pdfImage = Renderer.RenderHtmlAsPdf(html, fullpath);

                                                doc.AppendPdf(pdfImage);

                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e, "Unable to fully render attachments to pdf file - Last tried is '{0}'", lastFileTried);
                                }
                            }
                        }
                    }


                    // Last attach transcripts if requested
                    try
                    {
                        if (scholarship.TranscriptsRequired)
                        {
                            // TODO: Attach transcripts

                            int schoolYear = scholarship.PublishedDate.Year;
                            int currentMonth = scholarship.PublishedDate.Month;
                            if (currentMonth > 7)
                                schoolYear++;

                            string transcriptProcessPath = Path.Combine(transcriptPath, schoolYear + "");
                            string transcriptSavePath =
                                Path.Combine(transcriptProcessPath, app.Profile.StudentId + ".pdf");

                            if (File.Exists(transcriptSavePath))
                            {
                                var props = scholarship.ProfileProperties.Select(p => p.ProfileProperty?.PropertyKey).ToList();

                                // TODO: Merge in PDF at transcriptSavePath
                                PdfDocument pdfAttach = new PdfDocument(transcriptSavePath);

                                // Add an overlay on top of the transcript to hide the photo
                                pdfAttach.AddForegroundOverlayPdfToPage(0, pdfTranscriptPhotoOverlay);

                                if (!props.Contains("LastName"))
                                {
                                    // Add an overlay on top of the transcript to hide the student name
                                    pdfAttach.AddForegroundOverlayPdfToPage(0, pdfTranscriptNameOverlay);
                                }

                                doc.AppendPdf(pdfAttach);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Unable to attach transcript to application package");
                    }
                }

                doc.AddFooters(new SimpleHeaderFooter
                {
                    RightText = "Page {page} of {total-pages}",
                    FontSize = 10
                }, true);

                doc.SaveAs(joboutputPath);

                job.Ended = DateTime.Now;
                job.StatusMessage = "Completed";
                _context.Update(job);
                _context.SaveChanges();

            }

            Log.Information("Ending creation of application package");
        }
    }
}
