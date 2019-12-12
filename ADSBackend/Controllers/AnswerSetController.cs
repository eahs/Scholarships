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

        public async Task<IActionResult> QuestionFormAjax(int? id)  // id is an answerGroup
        {
            if (id == null)
                return NotFound();

            var agroup = await _context.AnswerGroup.Include(ag => ag.AnswerSets)
                                                   .ThenInclude(q => q.AnswerSet)
                                                   .ThenInclude(a => a.Answers)
                                                   .FirstOrDefaultAsync(ag => ag.AnswerGroupId == id);
                                               
            if (agroup == null)
            {
                return NotFound();
            }

            var asets = agroup.AnswerSets.Select(ag => ag.AnswerSet).ToList();

            QuestionSet qset = await _dataService.GetQuestionSet(asets.First().QuestionSetId);
            Profile profile = await _dataService.GetProfileAsync();

            if (qset == null)
            {
                return NotFound();
            }

            foreach (var aset in asets)
            {
                aset.Profile = profile;
            }
            qset.AnswerSets = asets;

            return View(qset);
        }

        private bool AnswerSetExists(int id)
        {
            return _context.AnswerSet.Any(e => e.AnswerSetId == id);
        }
    }
}
