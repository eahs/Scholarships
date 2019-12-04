using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Scholarship.Include(s => s.QuestionSet);
            return View(await applicationDbContext.ToListAsync());
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
        public IActionResult Create()
        {
            ViewData["QuestionSetId"] = new SelectList(_context.QuestionSet, "QuestionSetId", "Name");
            return View();
        }

        // POST: Scholarships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScholarshipId,SponsorCompany,SponsorName,SponsorAddress1,SponsorAddress2,SponsorPhone,SponsorEmail,Name,Description,Eligibility,Standards,Amount,ApplicationInstructions,ApplyOnline,TranscriptsRequired,ReleaseDate,DueDate,QuestionSetId")] Scholarship scholarship)
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
                return RedirectToAction(nameof(Index));
            }
            return View(scholarship);
        }

        // GET: Scholarships/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scholarship = await _context.Scholarship.FindAsync(id);
            if (scholarship == null)
            {
                return NotFound();
            }
            return View(scholarship);
        }

        // POST: Scholarships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScholarshipId,SponsorCompany,SponsorName,SponsorAddress1,SponsorAddress2,SponsorPhone,SponsorEmail,Name,Description,Eligibility,Standards,Amount,ApplicationInstructions,ApplyOnline,TranscriptsRequired,ReleaseDate,DueDate,QuestionSetId")] Scholarship scholarship)
        {
            if (id != scholarship.ScholarshipId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scholarship);
                    await _context.SaveChangesAsync();
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
            ViewData["QuestionSetId"] = new SelectList(_context.QuestionSet, "QuestionSetId", "Name", scholarship.QuestionSetId);
            return View(scholarship);
        }

        // GET: Scholarships/Delete/5
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

            return View(scholarship);
        }

        // POST: Scholarships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
