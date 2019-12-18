using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scholarships.Data;
using Scholarships.Models;
using Scholarships.Models.Forms;
using Scholarships.Services;

namespace Scholarships.Controllers
{
    public class AnswerGroupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public AnswerGroupController(ApplicationDbContext context, DataService dataService)
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

            int index = 0;
            foreach (var aset in asets)
            {
                aset.Profile = profile;
                aset.Index = index;

                if (aset.Answers.Count != qset.Questions.Count)
                {
                    foreach (var question in qset.Questions)
                    {
                        var exists = aset.Answers.FirstOrDefault(a => a.QuestionId == question.QuestionId);

                        if (exists == null)
                        {
                            Answer answer = new Answer
                            {
                                AnswerSetId = aset.AnswerSetId,
                                QuestionId = question.QuestionId,
                                Question = question                                
                            };
                            aset.Answers.Add(answer);
                        }
                    }
                }
                
                index++;
            }
            qset.AnswerSets = asets;



            return View(qset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<FormsBaseViewModel> Save(int? id, ICollection<AnswerSet> asets)
        {
            foreach (var aset in asets)
            {
                foreach (var answer in aset.Answers)
                {
                    
                }
            }

            return new FormsBaseViewModel
            {
                ErrorCode = QuestionSetError.NoError
            };
        }


        // See https://stackoverflow.com/questions/17759286/how-can-i-show-you-the-files-already-stored-on-server-using-dropzone-js for how to integrate already existing files into display
        // Scroll down to section that shows init function solution
        public async Task<ActionResult> Upload(IEnumerable<IFormFile> files, int questionid, int answersetid)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok();
        }

        private bool AnswerSetExists(int id)
        {
            return _context.AnswerSet.Any(e => e.AnswerSetId == id);
        }
    }
}
