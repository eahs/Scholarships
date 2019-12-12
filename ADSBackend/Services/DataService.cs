using Scholarships.Data;
using Scholarships.Models;
using Scholarships.Models.Identity;
using Scholarships.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Scholarships.Models.ScholarshipViewModels;
using System.ComponentModel.DataAnnotations;
using Scholarships.Models.Forms;

namespace Scholarships.Services
{
    public class DataService : IDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpContext _httpcontext;

        public DataService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontext = httpContextAccessor.HttpContext;            
        }

        public async Task<Profile> GetProfileAsync()
        {
            var user = await _userManager.GetUserAsync(_httpcontext.User);

            if (user == null)
                return null;

            var profile = await _context.Profile.Include(p => p.Guardians).FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (profile == null)
            {
                profile = new Profile
                {
                    UserId = user.Id,
                    Email = user.Email
                };
                _context.Profile.Add(profile);
                await _context.SaveChangesAsync();
            }

            return profile;
        }

        public async Task<List<ScholarshipFieldStatus>> VerifyApplicationStatus(Scholarship scholarship)
        {
            List<ScholarshipFieldStatus> status = new List<ScholarshipFieldStatus>();
            ICollection<ValidationResult> results = new List<ValidationResult>();

            Profile profile = await GetProfileAsync();
            ValidationContext vc = new ValidationContext(profile);
            bool isValid = Validator.TryValidateObject(profile, vc, results, true);

            foreach (var result in results)
            {
                List<string> MemberNames = result.MemberNames.ToList();
                if (MemberNames.Count > 0)
                {
                    string field = MemberNames[0];
                    ScholarshipProfileProperty prop = scholarship.ProfileProperties.FirstOrDefault(p => p.ProfileProperty.PropertyKey == field);

                    if (prop != null)
                    {
                        status.Add(new ScholarshipFieldStatus
                        {
                            FieldName = prop.ProfileProperty.PropertyName,
                            ErrorMessage = result.ErrorMessage.Replace(prop.ProfileProperty.PropertyKey, prop.ProfileProperty.PropertyName),
                            Validated = false,
                            FormURI = ProfileFieldToURI(MemberNames[0])
                        });
                    }
                }
            }

            return status;
        }


        // Remove any modelstate errors that don't pertain to the actual fields we are binding to
        public void ScrubModelState(string bindingFields, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary mState)
        {
            string[] bindingKeys = bindingFields.Split(",");
            foreach (string key in mState.Keys)
            {
                if (!bindingKeys.Contains(key))
                    mState.Remove(key);
            }
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

        public async Task<QuestionSet> GetQuestionSet(int QuestionSetId)
        {
            var qset = await _context.QuestionSet.Include(qs => qs.Questions).ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionSetId == QuestionSetId);

            // Fix sort orders
            if (qset != null)
                SortQuestionSet(qset);

            return qset;
        }

        public async Task<Application> GetApplication (int scholarshipId, int profileId, int questionSetId)
        {
            var application = await _context.Application.FirstOrDefaultAsync(app => app.ScholarshipId == scholarshipId && app.ProfileId == app.ProfileId);

            if (application == null)
            {
                // First we need to create a default answer set for the user
                var answerSet = new AnswerSet
                {
                    QuestionSetId = questionSetId,
                    ProfileId = profileId
                };
                _context.AnswerSet.Add(answerSet);
                await _context.SaveChangesAsync();

                // Create an answergroup, which is basically a generic container of answersets
                // Really we only use answerGroups for their unique id
                var answerGroup = new AnswerGroup();

                _context.AnswerGroup.Add(answerGroup);
                await _context.SaveChangesAsync();

                // Next create an answergroupset that will hold our first answerset
                var answerGroupSet = new AnswerGroupSets
                {
                    AnswerGroupId = answerGroup.AnswerGroupId,
                    AnswerSetId = answerSet.AnswerSetId
                };
                _context.AnswerGroupSets.Add(answerGroupSet);
                await _context.SaveChangesAsync();

                // Last, we need to create an application and attach the answer group
                application = new Application
                {
                    ProfileId = profileId,
                    ScholarshipId = scholarshipId,
                    AnswerGroupId = answerGroup.AnswerGroupId
                };
                _context.Application.Add(application);
                await _context.SaveChangesAsync();

            }

            return application;
        }

        private string ProfileFieldToURI (string fieldname)
        {
            return "/Profile/Edit";
        }
    }
}
