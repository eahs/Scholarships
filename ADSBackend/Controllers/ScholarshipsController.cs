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

namespace Scholarships.Controllers
{
    public class ScholarshipsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScholarshipsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Scholarships
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Index()
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

            var scholarship = await _context.Scholarship
                .Include(s => s.QuestionSet)
                .FirstOrDefaultAsync(m => m.ScholarshipId == id);
            if (scholarship == null)
            {
                return NotFound();
            }

            return View(scholarship);
        }

        // GET: Scholarships/Create
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            ViewData["QuestionSetId"] = new SelectList(_context.QuestionSet, "QuestionSetId", "Name");
            await SetupForm(new List<int>());

            return View();
        }

        // POST: Scholarships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("FieldsOfStudyIds, ScholarshipId,SponsorCompany,SponsorName,SponsorAddress1,SponsorAddress2,SponsorPhone,SponsorEmail,Name,Description,Eligibility,Standards,Amount,ApplicationInstructions,ApplyOnline,TranscriptsRequired,ReleaseDate,DueDate,QuestionSetId")] Scholarship scholarship)
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

        public async Task SetupForm (List<int> selectedFieldsOfStudyIds)
        {
            var fieldsOfStudy = await _context.FieldOfStudy.OrderBy(fos => fos.Name).ToListAsync();
            ViewBag.FieldsOfStudy = new MultiSelectList(fieldsOfStudy, "FieldOfStudyId", "Name", selectedFieldsOfStudyIds);
        }

        // GET: Scholarships/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarship = await _context.Scholarship.Include(s => s.FieldsOfStudy)
                                                        .ThenInclude(fos => fos.FieldOfStudy)
                                                        .FirstOrDefaultAsync(s => s.ScholarshipId == id);
            if (scholarship == null)
            {
                return NotFound();
            }

            scholarship.FieldsOfStudyIds = scholarship.FieldsOfStudy.Select(fos => fos.FieldOfStudyId).ToList();
            await SetupForm(scholarship.FieldsOfStudyIds);

            return View(scholarship);
        }

        // POST: Scholarships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("FieldsOfStudyIds,ScholarshipId,SponsorCompany,SponsorName,SponsorAddress1,SponsorAddress2,SponsorPhone,SponsorEmail,Name,Description,Eligibility,Standards,Amount,ApplicationInstructions,ApplyOnline,TranscriptsRequired,ReleaseDate,DueDate")] Scholarship scholarship)
        {
            if (id != scholarship.ScholarshipId)
            {
                return NotFound();
            }

            Scholarship _scholarship = await _context.Scholarship.Include(s => s.FieldsOfStudy)
                                                                 .ThenInclude(fos => fos.FieldOfStudy)
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

                    _context.Update(_scholarship);

                    await _context.SaveChangesAsync();

                    if (scholarship.FieldsOfStudyIds != null)
                    {
                        // Remove existing fields of study
                        var oldFieldsOfStudy = await _context.ScholarshipFieldOfStudy.Where(x => x.ScholarshipId == id).ToListAsync();
                        _context.ScholarshipFieldOfStudy.RemoveRange(oldFieldsOfStudy);
                        await _context.SaveChangesAsync();

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
                return RedirectToAction(nameof(Index));
            }

            _scholarship.FieldsOfStudyIds = _scholarship.FieldsOfStudy.Select(fos => fos.FieldOfStudyId).ToList();
            await SetupForm(_scholarship.FieldsOfStudyIds);

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
            await SetupForm(scholarship.FieldsOfStudyIds);

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
            return RedirectToAction(nameof(Index));
        }

        private bool ScholarshipExists(int id)
        {
            return _context.Scholarship.Any(e => e.ScholarshipId == id);
        }
    }
}
