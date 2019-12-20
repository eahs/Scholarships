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

        /// <summary>
        /// Returns a form built from a questionset
        /// </summary>
        /// <param name="id">AnswerGroupId from a previously constructed AnswerGroup</param>
        /// <returns></returns>
        public async Task<IActionResult> QuestionFormAjax(int? id)
        { 
            if (id == null)
                return NotFound();

            ViewBag.AnswerGroupId = id;  // The form still needs the answer group Id
            var qset = await _dataService.GetQuestionSetWithAnswers(ViewBag.AnswerGroupId);

            if (qset == null)
                return NotFound();

            return View(qset);
        }

        /// <summary>
        /// Saves an ajax submitted form from /Scholarships/Apply/{id}
        /// </summary>
        /// <param name="id">AnswerGroup Id</param>
        /// <param name="asets">List of partial answersets constructed from form data</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<FormsBaseViewModel> Save(int? id, ICollection<AnswerSet> asets)
        {
            if (id == null)
                return new FormsBaseViewModel { ErrorCode = QuestionSetError.NotFound };

            // First let's see if we can load the relevant question set
            var qset = await _dataService.GetQuestionSetWithAnswers((int)id);
            
            if (qset == null)
                return new FormsBaseViewModel { ErrorCode = QuestionSetError.NotFound };

            foreach (var aset in asets)
            {
                var _safeAnswerSet = qset.AnswerSets.Where(a => a.AnswerSetId == aset.AnswerSetId);

                // Check to see if the answerSet is already saved
                if (_safeAnswerSet == null)
                {
                    // TODO: Create an answerset to hold this answer?
                    continue;  // For now we don't have to worry about multiple answersets
                }

                // Iterate through the answers and save them 
                foreach (var answer in aset.Answers)
                {
                    // Each answer must be validated, answerId likely to be invalid
                    
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
