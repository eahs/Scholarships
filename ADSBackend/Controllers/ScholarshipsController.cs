using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Scholarships.Data;
using Scholarships.Models;
using Scholarships.Models.Forms;
using Scholarships.Models.ScholarshipViewModels;
using Scholarships.Services;
using Scholarships.Tasks;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Scholarships.Controllers
{
    public class ScholarshipsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;
        private readonly Services.Configuration Configuration;

        public ScholarshipsController(ApplicationDbContext context, DataService dataService, Services.Configuration configurationService)
        {
            _context = context;
            _dataService = dataService;
            Configuration = configurationService;
        }

        private async Task<ScholarshipListViewModel> FetchScholarships(ScholarshipListViewModel vm = null)
        {
            // First calculate start of school year so we only fetch scholarships from the current school year
            int startingYear = DateTime.Now.Month >= 7 ? DateTime.Now.Year : DateTime.Now.Year - 1;
            DateTime yearStart = new DateTime(startingYear, 7, 1);
            DateTime yearEnd = new DateTime(startingYear+1, 7, 1);

            // Grab only needed fields
            var scholarships = await _context.Scholarship
                .Where(s => s.Published && s.ReleaseDate >= yearStart && s.ReleaseDate < yearEnd)
                .Include(s => s.Categories)
                .Include(s => s.FieldsOfStudy)
                .Select(s => new Scholarship
                {
                    ScholarshipId = s.ScholarshipId,
                    Name = s.Name,
                    ReleaseDate = s.ReleaseDate,
                    DueDate = s.DueDate,
                    ApplyOnline = s.ApplyOnline,
                    Categories = s.Categories,
                    FieldsOfStudy = s.FieldsOfStudy
                })
                .ToListAsync();


            if (vm != null)
            {
                if (vm.FilterName?.Trim().Length > 0)
                    scholarships = scholarships.Where(s => s.Name.ToLower().Contains(vm.FilterName.ToLower())).ToList();

                if (vm.LocalOnly)
                {
                    scholarships = scholarships.Where(s => s.ApplyOnline).ToList();
                }

                if (vm.CategoryIds != null)
                {

                    scholarships = scholarships.Where(s =>
                                                        s.Categories.Select(x => x.CategoryId).ToList()
                                                                    .Intersect(vm.CategoryIds)
                                                                    .Any()
                                                        ).ToList();
                }

                if (vm.FieldsOfStudyIds != null)
                {
                    scholarships = scholarships.Where(s =>
                        s.FieldsOfStudy.Select(x => x.FieldOfStudyId).ToList()
                            .Intersect(vm.FieldsOfStudyIds)
                            .Any()
                    ).ToList();

                }
            }

            var profile = await _dataService.GetProfileAsync();

            await _dataService.IncludeFavorites(profile.ProfileId, scholarships);
            await _dataService.IncludeApplications(profile.ProfileId, scholarships);

            var fieldsOfStudy = await _context.FieldOfStudy.OrderBy(fos => fos.Name).ToListAsync();
            var categories = await _context.Category.OrderBy(prop => prop.Name).ToListAsync();

            if (vm == null)
            {
                vm = new ScholarshipListViewModel
                {
                    Scholarships = scholarships
                };
            }
            else
            {
                vm.Scholarships = scholarships;
            }

            vm.FilterCategory = new MultiSelectList(categories, "CategoryId", "Name",
                vm.CategoryIds ?? new List<int>());

            vm.FilterFieldsOfStudy = new MultiSelectList(fieldsOfStudy, "FieldOfStudyId", "Name",
                vm.FieldsOfStudyIds ?? new List<int>());


            return vm;
        }

        [Authorize]
        // GET: Scholarships
        public async Task<IActionResult> Index(ScholarshipListViewModel vm)
        {
            var scholarships = await FetchScholarships(vm);

            if (vm != null)
                scholarships.IsFiltered = true;

            return View(scholarships);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> PostIndex(ScholarshipListViewModel vm)
        {
            var scholarships = await FetchScholarships(vm);
            scholarships.IsFiltered = true;

            return View(scholarships);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int? id, bool state)
        {
            if (id == null)
                return Ok(new { Result = "NotFound" });

            var scholarship = await _context.Scholarship.FirstOrDefaultAsync(s => s.ScholarshipId == (int) id);
            var profile = await _dataService.GetProfileAsync();

            if (scholarship == null || profile == null)
                return Ok(new {Result = "NotFound"});

            ScholarshipFavorite favorite = new ScholarshipFavorite
            {
                ScholarshipId = scholarship.ScholarshipId,
                ProfileId = profile.ProfileId
            };

            if (state)
            {
                _context.AddOrUpdate(favorite);
            }
            else
            {
                _context.Remove(favorite);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET: Scholarships
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage()
        {
            var appsCompleted = await _context.Application.Where(app => app.Submitted)
                                                 .GroupBy(p => p.ScholarshipId)
                                                 .Select(x => new 
                                                    {
                                                        ScholarshipId = x.Key,
                                                        Count = x.Count()
                                                    }).ToDictionaryAsync(x => x.ScholarshipId, x => x.Count);

            var appsPending = await _context.Application.Where(app => !app.Submitted)
                .GroupBy(p => p.ScholarshipId)
                .Select(x => new
                {
                    ScholarshipId = x.Key,
                    Count = x.Count()
                }).ToDictionaryAsync(x => x.ScholarshipId, x => x.Count);

            var scholarships = await _context.Scholarship.Select(s => new Scholarship
            {
                ScholarshipId = s.ScholarshipId,
                PublishedDate = s.PublishedDate,
                Name = s.Name,
                DueDate = s.DueDate,
                ReleaseDate = s.ReleaseDate,
                ApplicantCount = appsCompleted.ContainsKey(s.ScholarshipId) ? appsCompleted[s.ScholarshipId] : 0,
                ApplicantPending = appsPending.ContainsKey(s.ScholarshipId) ? appsPending[s.ScholarshipId] : 0
            }).OrderByDescending(s => s.ReleaseDate).ToListAsync();

            return View(scholarships);
        }

        // GET: Scholarships/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarship = await _dataService.GetScholarship((int)id);
            if (scholarship == null)
            {
                return NotFound();
            }

            if (!scholarship.Published && User.IsInRole("Student"))
            {
                return NotFound();
            }

            var profile = await _dataService.GetProfileAsync();
            var app = await _context.Application.FirstOrDefaultAsync(a => a.ProfileId == profile.ProfileId && a.ScholarshipId == scholarship.ScholarshipId );
            var favorite = await _context.ScholarshipFavorite.FirstOrDefaultAsync(fav =>
                fav.ScholarshipId == scholarship.ScholarshipId && fav.ProfileId == profile.ProfileId);

            scholarship.IsFavorite = favorite != null;

            List<ScholarshipFieldStatus> fieldStatus = await _dataService.VerifyApplicationStatus(scholarship);

            ScholarshipDetailsViewModel vm = new ScholarshipDetailsViewModel
            {
                Scholarship = scholarship, 
                ApplicationCompleted = app != null ? app.Submitted : false,
                ProfileCompleted = fieldStatus.Count == 0,
                FieldStatus = fieldStatus,
                CanApply = (User.IsInRole("Student") || User.IsInRole("Admin")) && scholarship.ApplyOnline
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Applications(int id) // id = ScholarshipId
        {
            Scholarship scholarship = await _context.Scholarship.FirstOrDefaultAsync(s => s.ScholarshipId == id);

            if (scholarship == null)
                return NotFound();

            List<Application> applications = await _context.Application.Include(ap => ap.Profile)
                                                                       .Where(s => s.ScholarshipId == id && s.Submitted)
                                                                       .ToListAsync();

            ApplicationViewModel vm = new ApplicationViewModel
            {
                Scholarship = scholarship,
                Applications = applications
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin,Manager")]
        [Produces("application/json")]
        public async Task<object> ApplicationPackageGenerate(int? id, bool trigger = false) // id = ScholarshipId
        {
            var statusMessage = "Running";

            if (id == null)
                return new { error = true, message = "Scholarship Id not specified" };

            var job = await _context.Job.OrderByDescending(x => x.JobId).FirstOrDefaultAsync(j =>
                j.Type == "applications" && j.ForeignKey == (int) id);

            if (job != null && job.Completed)
            {
                if (job.StatusMessage == "Failed")
                    job = null;
                else if (job.StatusMessage == "Running")
                {
                    TimeSpan ts = DateTime.Now - job.Started;
                    if (ts.Minutes >= 10)
                    {
                        job.StatusMessage = "Failed";
                        job.Ended = DateTime.Now;

                        _context.Update(job);
                        _context.SaveChanges();
                        job = null;
                    }
                }
                else if (job.StatusMessage == "Completed")
                {
                    TimeSpan ts = DateTime.Now - job.Started;
                    if (ts.Minutes >= 5)
                    {
                        if (trigger)
                            job = null;
                    }

                }
            }

            if (job == null)
            {
                job = new Job
                {
                    Completed = false,
                    Created = DateTime.Now,
                    Type = "applications",
                    ForeignKey = (int)id,
                    StatusMessage = trigger ? "Pending" : ""
                };

                if (trigger)
                {
                    _context.Job.Add(job);
                    _context.SaveChanges();
                }

                BackgroundJob.Enqueue<ICreateApplicationPackage>(queue => queue.Execute());
            }

            statusMessage = job.StatusMessage;

            return new
            {
                error = false,
                message = statusMessage,
                job
            };

        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ApplicationPackageDownload(int id)  // id = job id
        {
            var job = await _context.Job.FirstOrDefaultAsync(x => x.JobId == id);

            if (job == null || job.StatusMessage != "Completed")
                return NotFound();

            var filePath = System.IO.Path.Combine(Configuration.ConfigPath.JobOutputPath,
                "job" + job.JobId + ".pdf");

            try
            {
                var stream = new FileStream(filePath, FileMode.Open);
                return new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception e)
            {
                Log.Error(e, "Error downloading file: {0}", filePath);
                return NotFound();
            }

        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ApplicationView(int? id) // id = ApplicationId
        {
            if (id == null)
                return NotFound();

            var vm = await _dataService.GetScholarshipApplicationViewModel((int)id);

            if (vm == null)
                return NotFound();

            return View(vm);

        }



        [Authorize(Roles = "Admin,Manager,Student")]
        public async Task<IActionResult> Apply (int? id)  // id of scholarship
        {
            if (id == null)
                return NotFound();

            var scholarship = await _dataService.GetScholarship((int)id);
            if (scholarship == null)
            {
                return NotFound();
            }

            if (!scholarship.Published && User.IsInRole("Student"))
            {
                return NotFound();
            }

            var profile = await _dataService.GetProfileAsync();

            Application app = await _dataService.GetApplication(scholarship.ScholarshipId, profile.ProfileId, scholarship.QuestionSetId);
            app.Profile = profile;

            if (app.Submitted)
            {
                return RedirectToAction("Index");
            }

            ScholarshipApplyViewModel vm = new ScholarshipApplyViewModel
            {
                Scholarship = scholarship,
                Application = app
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin,Manager,Student")]
        [Produces("application/json")]
        public async Task<FormsBaseViewModel> SaveApplication(Application _app)  // id of scholarship
        {
            if (_app.ScholarshipId == 0)
                return new FormsBaseViewModel
                {
                    ErrorCode = QuestionSetError.NotFound
                };

            var scholarship = await _dataService.GetScholarship(_app.ScholarshipId);
            if (scholarship == null)
            {
                return new FormsBaseViewModel
                {
                    ErrorCode = QuestionSetError.NotFound
                };
            }

            var profile = await _dataService.GetProfileAsync();

            Log.Information("[ScholarshipsController.SaveApplication] Saving Application for {0} {1} - Profile Id # {2}", profile.FirstName ?? "??", profile.LastName ?? "??", profile.ProfileId);

            Application app = await _dataService.GetApplication(scholarship.ScholarshipId, profile.ProfileId, scholarship.QuestionSetId);

            // Check to see if they haven't yet submitted it and it is prior to 11:59PM on the day the scholarship is due
            if (!app.Submitted && DateTime.Now <= scholarship.DueDate.Date.AddDays(1).AddMinutes(-1))
            {
                profile.Favorites = new List<ScholarshipFavorite>();

                app.Profile = profile;

                app.SubmittedDate = DateTime.Now;
                app.SubmissionStage = 2;
                app.Submitted = true;
                app.AcceptRelease = _app.AcceptRelease;
                app.Signature = _app.Signature;
                app.SignatureDate = _app.SignatureDate;
                app.ProfileSnapshot = JsonConvert.SerializeObject(profile, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                _context.Update(app);
                await _context.SaveChangesAsync();

                Log.Information("[ScholarshipsController.SaveApplication] Finished saved for {0} {1} - Profile Id # {2}", profile.FirstName ?? "??", profile.LastName ?? "??", profile.ProfileId);

            }

            return new FormsBaseViewModel
            {
                ErrorCode = QuestionSetError.NoError
            };
        }

        // GET: Scholarships/Create
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            ViewData["QuestionSetId"] = new SelectList(_context.QuestionSet, "QuestionSetId", "Name");

            await SetupForm(new List<int>(), new List<int>(), new List<int>());

            return View();
        }

        // POST: Scholarships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("FieldsOfStudyIds,ProfilePropertyIds,CategoryIds,ScholarshipId,DistrictMaintained,IncomeVerificationRequired,NumberOfYears,SponsorCompany,SponsorName,SponsorAddress1,SponsorAddress2,SponsorPhone,SponsorEmail,Name,Description,Eligibility,Standards,Amount,ApplicationInstructions,ApplyOnline,TranscriptsRequired,ReleaseDate,DueDate,QuestionSetId,Published")] Scholarship scholarship)
        {
            if (ModelState.IsValid)
            {
                QuestionSet qset = new QuestionSet
                {
                    Name = scholarship.Name ?? ""
                };
                _context.Add(qset);
                await _context.SaveChangesAsync();

                scholarship.QuestionSetId = qset.QuestionSetId;
                scholarship.ApplicationInstructions ??= "";
                scholarship.Amount ??= "";
                scholarship.Description ??= "";
                scholarship.Standards ??= "";
                scholarship.Eligibility ??= "";
                
                _context.Add(scholarship);
                await _context.SaveChangesAsync();

                if (scholarship.FieldsOfStudyIds != null)
                {
                    // Add new fields of study
                    var newFieldsOfStudy = scholarship.FieldsOfStudyIds.Select(x => new ScholarshipFieldOfStudy
                    {
                        ScholarshipId = scholarship.ScholarshipId,
                        FieldOfStudyId = x
                    });

                    if (newFieldsOfStudy != null)
                        _context.ScholarshipFieldOfStudy.AddRange(newFieldsOfStudy);
                }


                if (scholarship.ProfilePropertyIds != null)
                {
                    // Add new fields of study
                    var newProfileProperties = scholarship.ProfilePropertyIds.Select(x => new ScholarshipProfileProperty
                    {
                        ScholarshipId = scholarship.ScholarshipId,
                        ProfilePropertyId = x
                    });
                    if (newProfileProperties != null)
                        _context.ScholarshipProfileProperty.AddRange(newProfileProperties);

                }

                if (scholarship.CategoryIds != null)
                {
                    // Add new categories
                    var newCategories = scholarship.CategoryIds.Select(x => new ScholarshipCategory
                    {
                        ScholarshipId = scholarship.ScholarshipId,
                        CategoryId = x
                    });
                    if (newCategories != null)
                        _context.ScholarshipCategory.AddRange(newCategories);

                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Manage", "Scholarships");
            }
            return View(scholarship);
        }

        /// <summary>
        /// Sets up the ViewBag to include a multiselectlist of the current fields of study
        /// </summary>
        /// <param name="selectedFieldsOfStudyIds"></param>
        /// <returns></returns>
        private async Task SetupForm (List<int> selectedFieldsOfStudyIds, List<int> selectedProfilePropertyIds, List<int> selectedCategoryIds)
        {
            var fieldsOfStudy = await _context.FieldOfStudy.OrderBy(fos => fos.Name).ToListAsync();
            ViewBag.FieldsOfStudy = new MultiSelectList(fieldsOfStudy, "FieldOfStudyId", "Name", selectedFieldsOfStudyIds);

            var profileProperties = await _context.ProfileProperty.OrderBy(prop => prop.ProfilePropertyId).ToListAsync();
            ViewBag.ProfileProperties = new MultiSelectList(profileProperties, "ProfilePropertyId", "PropertyName",
                selectedProfilePropertyIds);

            var categories = await _context.Category.OrderBy(prop => prop.Name).ToListAsync();
            ViewBag.Categories = new MultiSelectList(categories, "CategoryId", "Name",
                selectedCategoryIds);

        }

        // GET: Scholarships/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarship = await _dataService.GetScholarship((int) id);
            if (scholarship == null)
            {
                return NotFound();
            }

            scholarship.FieldsOfStudyIds = scholarship.FieldsOfStudy.Select(fos => fos.FieldOfStudyId).ToList();
            scholarship.ProfilePropertyIds = scholarship.ProfileProperties.Select(prop => prop.ProfilePropertyId).ToList();
            scholarship.CategoryIds = scholarship.Categories.Select(prop => prop.CategoryId).ToList();

            await SetupForm(scholarship.FieldsOfStudyIds, scholarship.ProfilePropertyIds, scholarship.CategoryIds);

            return View(scholarship);
        }

        // POST: Scholarships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("FieldsOfStudyIds,ProfilePropertyIds,CategoryIds,ScholarshipId,DistrictMaintained,IncomeVerificationRequired,NumberOfYears,SponsorCompany,SponsorName,SponsorAddress1,SponsorAddress2,SponsorPhone,SponsorEmail,Name,Description,Eligibility,Standards,Amount,ApplicationInstructions,ApplyOnline,TranscriptsRequired,Published,ReleaseDate,DueDate")] Scholarship scholarship)
        {
            if (id != scholarship.ScholarshipId)
            {
                return NotFound();
            }

            Scholarship _scholarship = await _context.Scholarship.Include(s => s.FieldsOfStudy)
                                                                 .ThenInclude(fos => fos.FieldOfStudy)
                                                                 .Include(p => p.ProfileProperties).ThenInclude(prop => prop.ProfileProperty)
                                                                 .Include(p => p.Categories)
                                                                 .FirstOrDefaultAsync(s => s.ScholarshipId == id);

            if (ModelState.IsValid && _scholarship != null)
            {
                try
                {
                    _scholarship.SponsorCompany = scholarship.SponsorCompany ?? "";
                    _scholarship.SponsorName = scholarship.SponsorName ?? "";
                    _scholarship.SponsorAddress1 = scholarship.SponsorAddress1 ?? "";
                    _scholarship.SponsorAddress2 = scholarship.SponsorAddress2 ?? "";
                    _scholarship.SponsorPhone = scholarship.SponsorPhone ?? "";
                    _scholarship.SponsorEmail = scholarship.SponsorEmail ?? "";
                    _scholarship.Name = scholarship.Name ?? "";
                    _scholarship.Description = scholarship.Description ?? "";
                    _scholarship.Eligibility = scholarship.Eligibility ?? "";
                    _scholarship.Standards = scholarship.Standards ?? "";
                    _scholarship.Amount = scholarship.Amount ?? "";
                    _scholarship.ApplicationInstructions = scholarship.ApplicationInstructions ?? "";
                    _scholarship.ApplyOnline = scholarship.ApplyOnline;
                    _scholarship.TranscriptsRequired = scholarship.TranscriptsRequired;
                    _scholarship.ReleaseDate = scholarship.ReleaseDate;
                    _scholarship.DueDate = scholarship.DueDate;
                    _scholarship.DistrictMaintained = scholarship.DistrictMaintained;
                    _scholarship.NumberOfYears = scholarship.NumberOfYears;
                    _scholarship.IncomeVerificationRequired = scholarship.IncomeVerificationRequired;
                    _scholarship.Published = scholarship.Published;

                    _context.Update(_scholarship);

                    await _context.SaveChangesAsync();

                    // Remove existing fields of study
                    var oldFieldsOfStudy = await _context.ScholarshipFieldOfStudy.Where(x => x.ScholarshipId == id).ToListAsync();
                    _context.ScholarshipFieldOfStudy.RemoveRange(oldFieldsOfStudy);

                    var oldProfileProperties = await _context.ScholarshipProfileProperty.Where(x => x.ScholarshipId == id).ToListAsync();
                    _context.ScholarshipProfileProperty.RemoveRange(oldProfileProperties);

                    var oldCategories = await _context.ScholarshipCategory.Where(x => x.ScholarshipId == id).ToListAsync();
                    _context.ScholarshipCategory.RemoveRange(oldCategories);

                    await _context.SaveChangesAsync();

                    if (scholarship.FieldsOfStudyIds != null)
                    {
                        // Add new fields of study
                        var newFieldsOfStudy = scholarship.FieldsOfStudyIds.Select(x => new ScholarshipFieldOfStudy
                        {
                            ScholarshipId = _scholarship.ScholarshipId,
                            FieldOfStudyId = x
                        });
                        if (newFieldsOfStudy != null)
                            _context.ScholarshipFieldOfStudy.AddRange(newFieldsOfStudy);

                        await _context.SaveChangesAsync();
                    }

                    if (scholarship.ProfilePropertyIds != null)
                    {
                        // Add new fields of study
                        var newProfileProperties = scholarship.ProfilePropertyIds.Select(x => new ScholarshipProfileProperty
                        {
                            ScholarshipId = _scholarship.ScholarshipId,
                            ProfilePropertyId = x
                        });
                        if (newProfileProperties != null)
                            _context.ScholarshipProfileProperty.AddRange(newProfileProperties);

                        await _context.SaveChangesAsync();
                    }

                    if (scholarship.CategoryIds != null)
                    {
                        // Add new fields of study
                        var newCategories = scholarship.CategoryIds.Select(x => new ScholarshipCategory
                        {
                            ScholarshipId = scholarship.ScholarshipId,
                            CategoryId = x
                        });
                        if (newCategories != null)
                            _context.ScholarshipCategory.AddRange(newCategories);
                        await _context.SaveChangesAsync();

                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScholarshipExists(scholarship.ScholarshipId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Manage));
            }

            _scholarship.FieldsOfStudyIds = _scholarship.FieldsOfStudy.Select(fos => fos.FieldOfStudyId).ToList();
            _scholarship.ProfilePropertyIds = scholarship.ProfileProperties.Select(prop => prop.ProfilePropertyId).ToList();
            _scholarship.CategoryIds = scholarship.Categories.Select(prop => prop.CategoryId).ToList();

            await SetupForm(_scholarship.FieldsOfStudyIds, _scholarship.ProfilePropertyIds, _scholarship.CategoryIds);

            return View(_scholarship);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Duplicate(int id)
        {
            var _scholarship = await _dataService.DuplicateScholarshipAsync(id);

            return RedirectToAction("Edit", new { id = _scholarship.ScholarshipId });
        }

        // GET: Scholarships/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarship = await _dataService.GetScholarship((int) id);

            if (scholarship == null)
            {
                return NotFound();
            }

            scholarship.FieldsOfStudyIds = scholarship.FieldsOfStudy.Select(fos => fos.FieldOfStudyId).ToList();
            scholarship.ProfilePropertyIds = scholarship.ProfileProperties.Select(prop => prop.ProfilePropertyId).ToList();
            scholarship.CategoryIds = scholarship.Categories.Select(prop => prop.CategoryId).ToList();

            await SetupForm(scholarship.FieldsOfStudyIds, scholarship.ProfilePropertyIds, scholarship.CategoryIds);

            return View(scholarship);
        }

        // POST: Scholarships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scholarship = await _context.Scholarship.FindAsync(id);
            _context.Scholarship.Remove(scholarship);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }

        private bool ScholarshipExists(int id)
        {
            return _context.Scholarship.Any(e => e.ScholarshipId == id);
        }
    }
}
