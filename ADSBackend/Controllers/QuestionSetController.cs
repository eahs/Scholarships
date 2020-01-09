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
    [Authorize(Roles ="Admin,Manager")]
    public class QuestionSetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ViewRenderService _renderService;
        private readonly DataService _dataService;

        public QuestionSetController(ApplicationDbContext context, ViewRenderService renderService, DataService dataService)
        {
            _context = context;
            _renderService = renderService;
            _dataService = dataService;
        }

        public bool UserCanModifyQuestionSet(QuestionSet qset)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
                return true;

            return false;
        }

        public QuestionSetViewModel Error(QuestionSetError code)
        {
            return new QuestionSetViewModel { ErrorCode = code, ModelStateErrors = ModelState.Values.Where(i => i.Errors.Count > 0).ToList() };
        }

        // Admin level
        public async Task<IActionResult> Edit(int id)
        {
            var qset = await _dataService.GetQuestionSet(id);

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
            var qset = await _context.QuestionSet.FirstOrDefaultAsync(qs => qs.QuestionSetId == id);

            if (qset == null)
                return new QuestionViewModel {ErrorCode = QuestionSetError.NotFound};

            if (!UserCanModifyQuestionSet(qset))
                return new QuestionViewModel { ErrorCode = QuestionSetError.NotAuthorized };

            int index = await _context.Question.Where(qs => qs.QuestionSetId == id).CountAsync();

            Question q = new Question
            {
                QuestionSetId = qset.QuestionSetId,
                Name = "",
                Description = "",
                ErrorMessage = "",
                Config = "{}",
                Options = new List<QuestionOption>(),
                Required = false,
                Type = QuestionType.MultipleChoice,
                Order = index
            };

            _context.Add(q);
            await _context.SaveChangesAsync();


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

            qset = await _dataService.GetQuestionSet(id);

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

            var question = await _context.Question.Include(qs => qs.QuestionSet).FirstOrDefaultAsync(qs => qs.QuestionId == id);

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

            int index = await _context.QuestionOption.Where(qs => qs.QuestionId == id).CountAsync() - 1;

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
        public async Task<QuestionOptionViewModel> RemoveQuestionOption(int id, [Bind("QuestionIndex")] QuestionOptionViewModel qvm)  // id is of the question option
        {
            if (qvm.QuestionIndex < 0)
                return new QuestionOptionViewModel { ErrorCode = QuestionSetError.FormIndexNotProvided };

            var questionoption = await _context.QuestionOption.FirstOrDefaultAsync(qo => qo.QuestionOptionId == id);

            var question = await _context.Question.Include(qs => qs.QuestionSet)
                                                  .Include(qs => qs.Options)
                                                  .FirstOrDefaultAsync(qs => qs.QuestionId == questionoption.QuestionId);

            if (question == null)
                return new QuestionOptionViewModel { ErrorCode = QuestionSetError.NotFound };

            if (!UserCanModifyQuestionSet(question.QuestionSet))
                return new QuestionOptionViewModel { ErrorCode = QuestionSetError.NotAuthorized };

            QuestionOption q = new QuestionOption
            {
                QuestionOptionId = questionoption.QuestionOptionId
            };

            _context.QuestionOption.Remove(question.Options.Find(qo => qo.QuestionOptionId == questionoption.QuestionOptionId));

            question.Options.RemoveAt(question.Options.FindIndex(qo => qo.QuestionOptionId == questionoption.QuestionOptionId));

            await _context.SaveChangesAsync();

            question.Options = question.Options.OrderBy(qo => qo.Order).ToList();

            for (int index = 0; index < question.Options.Count; index++)
            {
                qvm.Index = index;
                qvm.ErrorCode = QuestionSetError.NoError;
                qvm.QuestionOption = question.Options[index];
                qvm.QuestionOption.Question = null;
                qvm.QuestionOptionForm += await _renderService.RenderToStringAsync("_QuestionOptionEditPartial", qvm);
            }

            return qvm;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<object> SaveQuestionOrder(int id, [Bind("order")] List<int> order)
        {
            QuestionSetError error = QuestionSetError.NoError;

            var questions = await _context.Question.Include(q => q.QuestionSet)
                                                  .Where(q => q.QuestionSetId == id)
                                                  .ToListAsync();

            if (questions == null)
                return new FormsBaseViewModel { ErrorCode = QuestionSetError.NotFound };

            var qset = questions.FirstOrDefault().QuestionSet;

            if (!UserCanModifyQuestionSet(qset))
                return new FormsBaseViewModel { ErrorCode = QuestionSetError.NotAuthorized };

            if (order.Count != questions.Count)
                return new FormsBaseViewModel { ErrorCode = QuestionSetError.NotAuthorized };

            for (int i = 0; i < order.Count; i++)
            {
                var question = questions.FirstOrDefault(q => q.QuestionId == order[i]);
                if (question != null)
                    question.Order = i;
            }
            _context.Question.UpdateRange(questions);
            await _context.SaveChangesAsync();

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

            var question = await _context.Question.Include(q => q.QuestionSet)
                                                  .Include(q => q.Options)
                                                  .FirstOrDefaultAsync(q => q.QuestionId == id);

            if (question == null)
                return new FormsBaseViewModel { ErrorCode = QuestionSetError.NotFound };

            var qset = question.QuestionSet;

            if (!UserCanModifyQuestionSet(qset))
                return new FormsBaseViewModel { ErrorCode = QuestionSetError.NotAuthorized };

            for (int i = 0; i < order.Count; i++)
            {
                var option = question.Options.FirstOrDefault(q => q.QuestionOptionId == order[i]);
                if (option != null)
                    option.Order = i;
            }

            _context.UpdateRange(question.Options);
            await _context.SaveChangesAsync();

            return new FormsBaseViewModel
            {
                ErrorCode = error
            };
        }

    }
}