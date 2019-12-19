using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scholarships.Data;
using Scholarships.Models;
using Scholarships.Models.Forms;
using Scholarships.Models.ScholarshipViewModels;
using Scholarships.Services;

namespace Scholarships.Controllers
{
    public class ScholarshipsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public ScholarshipsController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        // GET: Scholarships
        public async Task<IActionResult> Index()
        {
            var scholarships = await _context.Scholarship.Include(s => s.QuestionSet).ToListAsync();
            return View(scholarships);
        }

        // GET: Scholarships
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Manage()
        {
            var scholarships = await _context.Scholarship.Include(s => s.QuestionSet).ToListAsync();
            return View(scholarships);
        }

        // GET: Scholarships/Details/5
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

            List<ScholarshipFieldStatus> fieldStatus = await _dataService.VerifyApplicationStatus(scholarship);

            ScholarshipDetailsViewModel vm = new ScholarshipDetailsViewModel
            {
                Scholarship = scholarship,
                ApplicationCompleted = fieldStatus.Count == 0,
                FieldStatus = fieldStatus,
                CanApply = (User.IsInRole("Student") || User.IsInRole("Admin")) && scholarship.ApplyOnline
            };

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

            var profile = await _dataService.GetProfileAsync();

            Application app = await _dataService.GetApplication(scholarship.ScholarshipId, profile.ProfileId, scholarship.QuestionSetId);
            app.Profile = profile;

            ScholarshipApplyViewModel vm = new ScholarshipApplyViewModel
            {
                Scholarship = scholarship,
                Application = app
            };

            return View(vm);
        }

        // GET: Scholarships/Create
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            ViewData["QuestionSetId"] = new SelectList(_context.QuestionSet, "QuestionSetId", "Name");

            await SetupForm(new List<int>(), new List<int>());

            return View();
        }

        // POST: Scholarships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("FieldsOfStudyIds,ProfilePropertyIds,ScholarshipId,DistrictMaintained,IncomeVerificationRequired,NumberOfYears,SponsorCompany,SponsorName,SponsorAddress1,SponsorAddress2,SponsorPhone,SponsorEmail,Name,Description,Eligibility,Standards,Amount,ApplicationInstructions,ApplyOnline,TranscriptsRequired,ReleaseDate,DueDate,QuestionSetId")] Scholarship scholarship)
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

                _context.Add(scholarship);
                await _context.SaveChangesAsync();

                // Add new fields of study
                var newFieldsOfStudy = scholarship.FieldsOfStudyIds.Select(x => new ScholarshipFieldOfStudy
                {
                    ScholarshipId = scholarship.ScholarshipId,
                    FieldOfStudyId = x
                });

                if (newFieldsOfStudy != null)
                    _context.ScholarshipFieldOfStudy.AddRange(newFieldsOfStudy);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(scholarship);
        }

        /// <summary>
        /// Sets up the ViewBag to include a multiselectlist of the current fields of study
        /// </summary>
        /// <param name="selectedFieldsOfStudyIds"></param>
        /// <returns></returns>
        public async Task SetupForm (List<int> selectedFieldsOfStudyIds, List<int> selectedProfilePropertyIds)
        {
            var fieldsOfStudy = await _context.FieldOfStudy.OrderBy(fos => fos.Name).ToListAsync();
            ViewBag.FieldsOfStudy = new MultiSelectList(fieldsOfStudy, "FieldOfStudyId", "Name", selectedFieldsOfStudyIds);

            var profileProperties = await _context.ProfileProperty.OrderBy(prop => prop.ProfilePropertyId).ToListAsync();
            ViewBag.ProfileProperties = new MultiSelectList(profileProperties, "ProfilePropertyId", "PropertyName",
                selectedProfilePropertyIds);
        }

        // GET: Scholarships/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarship = await _context.Scholarship.Include(s => s.FieldsOfStudy).ThenInclude(fos => fos.FieldOfStudy)
                                                        .Include(p => p.ProfileProperties).ThenInclude(prop => prop.ProfileProperty)
                                                        .FirstOrDefaultAsync(s => s.ScholarshipId == id);
            if (scholarship == null)
            {
                return NotFound();
            }

            scholarship.FieldsOfStudyIds = scholarship.FieldsOfStudy.Select(fos => fos.FieldOfStudyId).ToList();
            scholarship.ProfilePropertyIds = scholarship.ProfileProperties.Select(prop => prop.ProfilePropertyId).ToList();
            await SetupForm(scholarship.FieldsOfStudyIds, scholarship.ProfilePropertyIds);

            return View(scholarship);
        }

        // POST: Scholarships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("FieldsOfStudyIds,ProfilePropertyIds,ScholarshipId,DistrictMaintained,IncomeVerificationRequired,NumberOfYears,SponsorCompany,SponsorName,SponsorAddress1,SponsorAddress2,SponsorPhone,SponsorEmail,Name,Description,Eligibility,Standards,Amount,ApplicationInstructions,ApplyOnline,TranscriptsRequired,ReleaseDate,DueDate")] Scholarship scholarship)
        {
            if (id != scholarship.ScholarshipId)
            {
                return NotFound();
            }

            Scholarship _scholarship = await _context.Scholarship.Include(s => s.FieldsOfStudy)
                                                                 .ThenInclude(fos => fos.FieldOfStudy)
                                                                 .Include(p => p.ProfileProperties).ThenInclude(prop => prop.ProfileProperty)
                                                                 .FirstOrDefaultAsync(s => s.ScholarshipId == id);

            if (ModelState.IsValid && _scholarship != null)
            {
                try
                {
                    _scholarship.SponsorCompany = scholarship.SponsorCompany;
                    _scholarship.SponsorName = scholarship.SponsorName;
                    _scholarship.SponsorAddress1 = scholarship.SponsorAddress1;
                    _scholarship.SponsorAddress2 = scholarship.SponsorAddress2;
                    _scholarship.SponsorPhone = scholarship.SponsorPhone;
                    _scholarship.SponsorEmail = scholarship.SponsorEmail;
                    _scholarship.Name = scholarship.Name;
                    _scholarship.Description = scholarship.Description;
                    _scholarship.Eligibility = scholarship.Eligibility;
                    _scholarship.Standards = scholarship.Standards;
                    _scholarship.Amount = scholarship.Amount;
                    _scholarship.ApplicationInstructions = scholarship.ApplicationInstructions;
                    _scholarship.ApplyOnline = scholarship.ApplyOnline;
                    _scholarship.TranscriptsRequired = scholarship.TranscriptsRequired;
                    _scholarship.ReleaseDate = scholarship.ReleaseDate;
                    _scholarship.DueDate = scholarship.DueDate;
                    _scholarship.DistrictMaintained = scholarship.DistrictMaintained;
                    _scholarship.NumberOfYears = scholarship.NumberOfYears;
                    _scholarship.IncomeVerificationRequired = scholarship.IncomeVerificationRequired;

                    _context.Update(_scholarship);

                    await _context.SaveChangesAsync();

                    // Remove existing fields of study
                    var oldFieldsOfStudy = await _context.ScholarshipFieldOfStudy.Where(x => x.ScholarshipId == id).ToListAsync();
                    _context.ScholarshipFieldOfStudy.RemoveRange(oldFieldsOfStudy);

                    var oldProfileProperties = await _context.ScholarshipProfileProperty.Where(x => x.ScholarshipId == id).ToListAsync();
                    _context.ScholarshipProfileProperty.RemoveRange(oldProfileProperties);

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

            await SetupForm(_scholarship.FieldsOfStudyIds, _scholarship.ProfilePropertyIds);

            return View(_scholarship);
        }

        // GET: Scholarships/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarship = await _context.Scholarship
                .Include(s => s.QuestionSet)
                .FirstOrDefaultAsync(m => m.ScholarshipId == id);
            if (scholarship == null)
            {
                return NotFound();
            }

            scholarship.FieldsOfStudyIds = scholarship.FieldsOfStudy.Select(fos => fos.FieldOfStudyId).ToList();
            scholarship.ProfilePropertyIds = scholarship.ProfileProperties.Select(prop => prop.ProfilePropertyId).ToList();

            await SetupForm(scholarship.FieldsOfStudyIds, scholarship.ProfilePropertyIds);

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
