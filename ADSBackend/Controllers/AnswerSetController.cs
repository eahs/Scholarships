using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scholarships.Data;
using Scholarships.Models.Forms;
using Scholarships.Services;

namespace Scholarships.Controllers
{
    public class AnswerSetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public AnswerSetController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        // GET: AnswerSet
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AnswerSet.Include(a => a.Profile).Include(a => a.QuestionSet);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> QuestionFormAjax(int? id)  // id of answerSet
        {
            if (id == null)
                return NotFound();

            var aset = await _context.AnswerSet.FirstOrDefaultAsync(app => app.AnswerSetId == id);

            if (aset == null)
            {
                return NotFound();
            }

            QuestionSet qset = await _dataService.GetQuestionSet(aset.QuestionSetId);

            if (qset == null)
            {
                return NotFound();
            }

            aset.QuestionSet = qset;
            aset.Profile = await _dataService.GetProfileAsync();

            return View(aset);
        }

        private bool AnswerSetExists(int id)
        {
            return _context.AnswerSet.Any(e => e.AnswerSetId == id);
        }
    }
}
