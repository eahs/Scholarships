using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Scholarships.Data;
using Scholarships.Models.Forms;
using Scholarships.Services;
using Scholarships.Util;

namespace Scholarships.Controllers
{
    public class QuestionSetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IViewRenderService _renderService;

        public QuestionSetController(ApplicationDbContext context, IViewRenderService renderService)
        {
            _context = context;
            _renderService = renderService;
        }

        public bool UserCanModifyQuestionSet(QuestionSet qset)
        {
            if (User.IsInRole("Admin"))
                return true;

            return false;
        }

        public QuestionSetViewModel Error(QuestionSetError code)
        {
            return new QuestionSetViewModel { ErrorCode = code, ModelStateErrors = ModelState.Values.Where(i => i.Errors.Count > 0).ToList() };
        }

        private void SortQuestionSet(QuestionSet qset)
        {
            qset.Questions = qset.Questions.OrderBy(q => q.Order).ToList();

            foreach (var question in qset.Questions)
            {
                if (question.Options != null)
                {
                    question.Options = question.Options.OrderBy(qo => qo.Order).ToList();
                }
            }

        }

        // Remove any modelstate errors that don't pertain to the actual fields we are binding to
        private void ScrubModelState(string bindingFields)
        {
            string[] bindingKeys = bindingFields.Split(",");
            foreach (string key in ModelState.Keys)
            {
                if (!bindingKeys.Contains(key))
                    ModelState.Remove(key);
            }
        }

        // Admin level
        public async Task<IActionResult> Edit(int id)
        {
            var qset = await _context.QuestionSet.Include(qs => qs.Questions).ThenInclude(q => q.Options)
                                                 .FirstOrDefaultAsync(q => q.QuestionSetId == id);

            if (qset == null)
                return NotFound();

            if (!UserCanModifyQuestionSet(qset))
                return NotFound();

            // Fix sort orders
            SortQuestionSet(qset);

            var model = new QuestionSetViewModel { FormId = "12345", QuestionSet = qset };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<QuestionSetViewModel> Edit(int id, [Bind("Name,Description,SingularName,PluralName,AllowMultiple")] QuestionSet questionSet)
        {
            var qset = await _context.QuestionSet.FirstOrDefaultAsync(q => q.QuestionSetId == id);

            if (qset == null)
                return Error(QuestionSetError.NotFound);

            if (!UserCanModifyQuestionSet(qset))
                return Error(QuestionSetError.NotAuthorized);

            if (ModelState.IsValid)
            {
                qset.Name = questionSet.Name;
                qset.Description = questionSet.Description;
                qset.SingularName = questionSet.SingularName;
                qset.PluralName = questionSet.PluralName;
                qset.AllowMultiple = questionSet.AllowMultiple;

                _context.QuestionSet.Update(qset);
                await _context.SaveChangesAsync();

                return new QuestionSetViewModel { QuestionSet = qset };
            }

            return Error(QuestionSetError.InvalidForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<QuestionViewModel> EditQuestions(int id, [Bind("Name,Description,Type,Options,Options.Name,Options.Order")] List<Question> questions)
        {
            var qset = await _context.QuestionSet.FirstOrDefaultAsync(q => q.QuestionSetId == id);

            if (qset == null)
                return new QuestionViewModel { ErrorCode = QuestionSetError.NotFound };

            if (!UserCanModifyQuestionSet(qset))
                return new QuestionViewModel { ErrorCode = QuestionSetError.NotAuthorized };

            var _questions = await _context.Question.Where(q => q.QuestionSetId == qset.QuestionSetId).ToListAsync();

            int order = 0;
            foreach (Question question in questions)
            {
                var _question = _questions.FirstOrDefault(q => q.QuestionId == question.QuestionId && q.QuestionSetId == qset.QuestionSetId);

                if (_question != null)
                {
                    _question.Name = question.Name ?? "";
                    _question.Description = question.Description ?? "";
                    _question.Type = question.Type;

                    _context.Question.Update(_question);

                    if (question.Options != null)
                    {
                        for (int k = 0; k < question.Options.Count; k++)
                        {
                            question.Options[k].QuestionId = question.QuestionId;
                        }

                        _context.QuestionOption.UpdateRange(question.Options);
                    }

                }
            }

            await _context.SaveChangesAsync();

            return new QuestionViewModel { ErrorCode = QuestionSetError.NotFound };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<QuestionViewModel> AddQuestion(int id)  // id is of question set
        {
            var qset = await _context.QuestionSet.FirstOrDefaultAsync(q => q.QuestionSetId == id);

            if (qset == null)
                return new QuestionViewModel {ErrorCode = QuestionSetError.NotFound};

            if (!UserCanModifyQuestionSet(qset))
                return new QuestionViewModel { ErrorCode = QuestionSetError.NotAuthorized };

            Question q = new Question
            {
                QuestionSetId = qset.QuestionSetId,
                Name = "",
                Description = "",
                ErrorMessage = "",
                Config = "{}",
                Options = new List<QuestionOption>(),
                Required = false,
                Type = QuestionType.MultipleChoice
            };

            _context.Add(q);
            await _context.SaveChangesAsync();


            int index = await _context.Question.Where(q => q.QuestionSetId == id).CountAsync() - 1;

            var qvm = new QuestionViewModel
            {
                Index = index,
                ErrorCode = QuestionSetError.NoError,
                Question =  q
            };

            qvm.QuestionForm = await _renderService.RenderToStringAsync("_QuestionEditPartial", qvm);

            return qvm;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<object> SaveQuestionOrder(int id, [Bind("order")] List<int> order)
        {
            QuestionSetError error = QuestionSetError.NoError;

            return new FormsBaseViewModel
            {
                ErrorCode = error
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<FormsBaseViewModel> SaveQuestionOptionOrder(int id, [Bind("order")] List<int> order)
        {
            QuestionSetError error = QuestionSetError.NoError;

            return new FormsBaseViewModel
            {
                ErrorCode = error
            };
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,SingularName,PluralName,AllowMultiple")] QuestionSet questionSet)
        {
            var qset = await _context.QuestionSet.FirstOrDefaultAsync(q => q.QuestionSetId == id);

            if (qset == null)
                return NotFound();

            if (!UserCanModifyQuestionSet(qset))
                return NotFound();

            if (ModelState.IsValid)
            {
                qset.Name = questionSet.Name;
                qset.Description = questionSet.Description;
                qset.SingularName = questionSet.SingularName;
                qset.PluralName = questionSet.PluralName;
                qset.AllowMultiple = questionSet.AllowMultiple;

                _context.QuestionSet.Update(qset);
                await _context.SaveChangesAsync();
            }

            return View(qset);
        }
        */

        /*
                public async Task<QuestionSetViewModel> Edit(int id)
                {
                    var qset = await _context.QuestionSet.Include(qs => qs.Questions).ThenInclude(q => q.Options)
                                                         .FirstOrDefaultAsync(q => q.QuestionSetId == id);

                    if (qset == null)
                        return Error(QuestionSetError.NotFound);

                    if (!UserCanModifyQuestionSet(qset))
                        return new QuestionSetViewModel { ErrorCode = QuestionSetError.NotAuthorized };

                    // Fix sort orders
                    SortQuestionSet(qset);

                    var model = new QuestionSetViewModel { FormId = "12345", QuestionSet = qset };

                    var form = await _renderService.RenderToStringAsync("_EditQuestionSetPartial", model);
                    model.PrimaryForm = form;

                    return model;
                }

                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<QuestionSetViewModel> Edit(int id, [Bind("Name,Description,SingularName,PluralName,AllowMultiple")] QuestionSet questionSet)
                {
                    var qset = await _context.QuestionSet.FirstOrDefaultAsync(q => q.QuestionSetId == id);

                    if (qset == null)
                        return Error(QuestionSetError.NotFound);

                    if (!UserCanModifyQuestionSet(qset))
                        return Error(QuestionSetError.NotAuthorized);

                    if (ModelState.IsValid)
                    {
                        qset.Name = questionSet.Name;
                        qset.Description = questionSet.Description;
                        qset.SingularName = questionSet.SingularName;
                        qset.PluralName = questionSet.PluralName;
                        qset.AllowMultiple = questionSet.AllowMultiple;

                        _context.QuestionSet.Update(qset);
                        await _context.SaveChangesAsync();

                        return new QuestionSetViewModel { QuestionSet = qset };
                    }

                    return Error(QuestionSetError.InvalidForm);
                }

        */

    }
}