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

namespace Scholarships.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class FieldsOfStudyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FieldsOfStudyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FieldsOfStudy
        public async Task<IActionResult> Index()
        {
            return View(await _context.FieldOfStudy.OrderBy(fos => fos.Name).ToListAsync());
        }

        // GET: FieldsOfStudy/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await _context.FieldOfStudy
                .FirstOrDefaultAsync(m => m.FieldOfStudyId == id);
            if (fieldOfStudy == null)
            {
                return NotFound();
            }

            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudy/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FieldsOfStudy/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FieldOfStudyId,Name")] FieldOfStudy fieldOfStudy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fieldOfStudy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudy/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await _context.FieldOfStudy.FindAsync(id);
            if (fieldOfStudy == null)
            {
                return NotFound();
            }
            return View(fieldOfStudy);
        }

        // POST: FieldsOfStudy/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FieldOfStudyId,Name")] FieldOfStudy fieldOfStudy)
        {
            if (id != fieldOfStudy.FieldOfStudyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fieldOfStudy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FieldOfStudyExists(fieldOfStudy.FieldOfStudyId))
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
            return View(fieldOfStudy);
        }

        // GET: FieldsOfStudy/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fieldOfStudy = await _context.FieldOfStudy
                .FirstOrDefaultAsync(m => m.FieldOfStudyId == id);
            if (fieldOfStudy == null)
            {
                return NotFound();
            }

            return View(fieldOfStudy);
        }

        // POST: FieldsOfStudy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fieldOfStudy = await _context.FieldOfStudy.FindAsync(id);
            _context.FieldOfStudy.Remove(fieldOfStudy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FieldOfStudyExists(int id)
        {
            return _context.FieldOfStudy.Any(e => e.FieldOfStudyId == id);
        }
    }
}
