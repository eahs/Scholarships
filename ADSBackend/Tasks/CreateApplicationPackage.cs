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
using Scholarships.Models;
using Scholarships.Models.Forms;
using Scholarships.Models.ScholarshipViewModels;
using Scholarships.Services;
using Scholarships.Util;

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

            // TODO: REMOVE ME - TEMPORARY
            var njob = new Job
            {
                Completed = false,
                Created = DateTime.Now,
                Type = "applications",
                ForeignKey = 2
            };
            _context.Job.Add(njob);
            _context.SaveChanges();
            // TODO: END REMOVE ME

            var job = _context.Job.FirstOrDefault(j => !j.Completed && j.Type == "applications");

            if (job != null)
            {
                // Immediately mark the job as completed, hung or faulty jobs will be both completed and "Running"
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
                Renderer.PrintOptions.JpegQuality = 80;
                Renderer.PrintOptions.GrayScale = false;
                Renderer.PrintOptions.FitToPaperWidth = true;
                Renderer.PrintOptions.InputEncoding = Encoding.UTF8;
                Renderer.PrintOptions.Zoom = 100;
                Renderer.PrintOptions.CreatePdfFormsFromHtml = false;
                Renderer.PrintOptions.MarginTop = 40;  //millimeters
                Renderer.PrintOptions.MarginLeft = 20;  //millimeters
                Renderer.PrintOptions.MarginRight = 20;  //millimeters
                Renderer.PrintOptions.MarginBottom = 40;  //millimeters
                Renderer.PrintOptions.FirstPageNumber = 1; //use 2 if a coverpage  will be appended

                PdfDocument doc = Renderer.RenderHtmlAsPdf("");
                doc.RemovePage(0);

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
                    applicationPage.BasicProfile = await _viewRenderService.RenderToStringAsync("_scholarshipapplicationpartial", vm, "scholarships");

                    // Next render any additional questions that were provided
                    if (qset.Questions?.Count > 0)
                    {
                        // TODO: Render answers to questions
                        applicationPage.FormAnswers = await _viewRenderService.RenderToStringAsync("questionsetappviewpartial", vm.QuestionSet, "answergroup");

                    }

                    string finalApplication =
                        await _viewRenderService.RenderToStringAsync("_scholarshipapplicationfull", applicationPage,
                            "scholarships");

                    PdfDocument page = Renderer.RenderHtmlAsPdf(finalApplication, "https://scholarships.eastonsd.org/");

                    doc.AppendPdf(page);

                    // Last attach transcripts if requested
                    if (scholarship.TranscriptsRequired)
                    {
                        // TODO: Attach transcripts

                        int schoolYear = scholarship.PublishedDate.Year;
                        int currentMonth = scholarship.PublishedDate.Month;
                        if (currentMonth > 7)
                            schoolYear++;

                        string transcriptProcessPath = Path.Combine(transcriptPath, schoolYear + "");
                        string transcriptSavePath = Path.Combine(transcriptProcessPath, app.Profile.StudentId + ".pdf");

                        if (File.Exists(transcriptSavePath))
                        {
                            // TODO: Merge in PDF at transcriptSavePath
                        }
                    }
                }

                doc.SaveAs(joboutputPath);
            }


            /*
            string transcriptSourcePath = Path.Combine(transcriptPath, "transcripts.pdf");

            if (!File.Exists(transcriptSourcePath))
            {
                Log.Error("Executing GenerateTranscripts - Unable to find transcripts.pdf");
                return;
            }
            */

            // Calculate the current graduating year for seniors
            /*
            int schoolYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            if (currentMonth > 7)
                schoolYear++;

            string transcriptProcessPath = Path.Combine(transcriptPath, schoolYear + "");
            Directory.CreateDirectory(transcriptProcessPath);

            PdfDocument PDF = null;

            try
            {
                PDF = PdfDocument.FromFile(transcriptSourcePath);
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to open transcripts pdf");
                return;
            }
            */

            /*
                PdfDocument transcript = PDF.CopyPages(studentIndex[id]);

                string transcriptSavePath = Path.Combine(transcriptProcessPath, id + ".pdf");

                try
                {
                    transcript.SaveAs(transcriptSavePath);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unable to save pdf file: {0}", transcriptSavePath);
                }
            */

            Log.Information("Ending creation of application package");
        }
    }
}
