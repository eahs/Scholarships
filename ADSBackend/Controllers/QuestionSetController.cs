using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        private async Task<QuestionSet> GetQuestionSet(int QuestionSetId)
        {
            var qset = await _context.QuestionSet.Include(qs => qs.Questions).ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionSetId == QuestionSetId);

            // Fix sort orders
            if (qset != null)
                SortQuestionSet(qset);

            return qset;
        }

        // Admin level
        public async Task<IActionResult> Edit(int id)
        {
            var qset = await GetQuestionSet(id);

            if (qset == null)
                return NotFound();

            if (!UserCanModifyQuestionSet(qset))
                return NotFound();

            var model = new QuestionSetViewModel { FormId = "12345", QuestionSet = qset };

            List<QuestionViewModel> questionvm = new List<QuestionViewModel>();

            for (int i = 0; i < qset.Questions.Count; i++)
            {
                questionvm.Add(new QuestionViewModel
                {
                    Index = i,
                    Question = qset.Questions[i]
                });
            }

            model.Questions = questionvm;

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
            qvm.Question.QuestionSet.Questions = null;

            return qvm;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<QuestionViewModel> RemoveQuestion(int id, [Bind("QuestionId")] Question question)  // id is of question set
        {
            var qset = await _context.QuestionSet.FirstOrDefaultAsync(q => q.QuestionSetId == id);

            if (qset == null)
                return new QuestionViewModel { ErrorCode = QuestionSetError.NotFound };

            if (!UserCanModifyQuestionSet(qset))
                return new QuestionViewModel { ErrorCode = QuestionSetError.NotAuthorized };

            var _question = await _context.Question.FirstOrDefaultAsync(q => q.QuestionSetId == id && q.QuestionId == question.QuestionId);

            if (_question == null)
                return new QuestionViewModel { ErrorCode = QuestionSetError.NotFound };

            _context.Question.Remove(_question);
            await _context.SaveChangesAsync();

            qset = await GetQuestionSet(id);

            List<QuestionViewModel> questionvm = new List<QuestionViewModel>();

            for (int i = 0; i < qset.Questions.Count; i++)
            {
                questionvm.Add(new QuestionViewModel
                {
                    Index = i,
                    Question = qset.Questions[i]
                });
            }

            QuestionSetViewModel qvm = new QuestionSetViewModel
            {
                QuestionSet = qset,
                Questions = questionvm
            };

            return new QuestionViewModel
            {
                QuestionForm = await _renderService.RenderToStringAsync("_QuestionFormPartial", qvm)
            };
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<QuestionOptionViewModel> AddQuestionOption(int id, [Bind("QuestionIndex")] QuestionOptionViewModel qvm)  // id is of the question, qindex is sortable list index
        {
            if (qvm.QuestionIndex < 0)
                return new QuestionOptionViewModel {ErrorCode = QuestionSetError.FormIndexNotProvided};

            var question = await _context.Question.Include(q => q.QuestionSet).FirstOrDefaultAsync(q => q.QuestionId == id);

            if (question == null)
                return new QuestionOptionViewModel { ErrorCode = QuestionSetError.NotFound };

            var qset = question.QuestionSet;

            if (!UserCanModifyQuestionSet(qset))
                return new QuestionOptionViewModel { ErrorCode = QuestionSetError.NotAuthorized };

            QuestionOption q = new QuestionOption
            {
                QuestionId = question.QuestionId,
                Name = ""
            };

            _context.Add(q);
            await _context.SaveChangesAsync();

            int index = await _context.QuestionOption.Where(q => q.QuestionId == id).CountAsync() - 1;

            qvm.Index = index;
            qvm.ErrorCode = QuestionSetError.NoError;
            qvm.QuestionOption = q;
            qvm.QuestionOption.Question = null;
            qvm.QuestionOptionForm = await _renderService.RenderToStringAsync("_QuestionOptionEditPartial", qvm);

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

            var question = await _context.Question.Include(q => q.QuestionSet).FirstOrDefaultAsync(q => q.QuestionId == id);

            if (question == null)
                return new FormsBaseViewModel { ErrorCode = QuestionSetError.NotFound };

            var qset = question.QuestionSet;

            if (!UserCanModifyQuestionSet(qset))
                return new FormsBaseViewModel { ErrorCode = QuestionSetError.NotAuthorized };



            return new FormsBaseViewModel
            {
                ErrorCode = error
            };
        }

    }
}