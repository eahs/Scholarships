﻿using Scholarships.Data;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Scholarships.Models.Forms;
using Newtonsoft.Json;
using Serilog;

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

            Profile profile;

            profile = await _context.Profile.Include(p => p.Guardians)
                                            .Include(p => p.FieldOfStudy)
                                            .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                // Let's see if there is one based off of email address
                var iprofile = await _context.ImportedProfile.FirstOrDefaultAsync(p => p.Email == user.Email); 

                if (iprofile != null)
                {
                    // Claim this profile
                    var iprofilejson = JsonConvert.SerializeObject(iprofile);
                    profile = JsonConvert.DeserializeObject<Profile>(iprofilejson);
                    profile.UserId = user.Id;
                    profile.Email = "";  // We cannot use a roverkids.org email in this field

                    _context.Add(profile);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    profile = new Profile
                    {
                        UserId = user.Id,
                        Email = user.Email
                    };
                    _context.Profile.Add(profile);
                    await _context.SaveChangesAsync();

                }

            }

            return profile;
        }

        public async Task IncludeFavorites(int profileId, List<Scholarship> scholarships)
        {
            var favorites = await _context.ScholarshipFavorite.Where(p => p.ProfileId == profileId)
                .Select(x => x.ScholarshipId)
                .ToListAsync();

            foreach (var scholarship in scholarships)
            {
                if (favorites.Contains(scholarship.ScholarshipId))
                    scholarship.IsFavorite = true;
            }

            return;
        }

        public async Task IncludeApplications(int profileId, List<Scholarship> scholarships)
        {
            var applications = await _context.Application.Where(app => app.ProfileId == profileId && app.Submitted)
                                                                 .Select(x => x.ScholarshipId)
                                                                 .ToListAsync();

            scholarships.ForEach(s =>
            {
                if (applications.Contains(s.ScholarshipId))
                    s.HasApplied = true;
            });
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

        private static void SortQuestionSet(QuestionSet qset)
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

        public async Task<QuestionSet> GetQuestionSetWithAnswers (int AnswerGroupId)
        {
            var agroup = await _context.AnswerGroup.Include(ag => ag.AnswerSets)
                                                    .ThenInclude(q => q.AnswerSet)
                                                    .ThenInclude(a => a.Answers)
                                                    .ThenInclude(ans => ans.FileAttachmentGroup)
                                                    .ThenInclude(fa => fa.FileAttachments)
                                                    .FirstOrDefaultAsync(ag => ag.AnswerGroupId == AnswerGroupId);

            if (agroup == null)
                return null;

            var asets = agroup.AnswerSets.Select(ag => ag.AnswerSet).ToList();
            var firstSet = asets.First();

            Profile profile = await GetProfileAsync();

            if (firstSet == null || firstSet.ProfileId != profile.ProfileId)
                return null;
            
            QuestionSet qset = await GetQuestionSet(firstSet.QuestionSetId);
            
            if (qset == null)
                return null;

            int index = 0;
            foreach (var aset in asets)
            {
                aset.Profile = profile;
                aset.Index = index;

                if (aset.Answers.Count != qset.Questions.Count)
                {
                    foreach (var question in qset.Questions)
                    {
                        var exists = aset.Answers.FirstOrDefault(a => a.QuestionId == question.QuestionId && a.AnswerSetId == aset.AnswerSetId);

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
                else
                {
                    foreach (var answer in aset.Answers)
                    {
                        var config = JsonConvert.DeserializeObject<Answer>(answer.Config);

                        if (config.QuestionOptions != null)
                        {
                            answer.QuestionOptions = config.QuestionOptions;
                        }


                    }
                }

                index++;
            }
            qset.AnswerSets = asets;
            qset.AnswerGroupId = AnswerGroupId;

            return qset;
        }

        public async Task<Scholarship> GetScholarship (int scholarshipId)
        {
            var scholarship = await _context.Scholarship
                .Include(s => s.QuestionSet)
                .Include(s => s.ProfileProperties).ThenInclude(s => s.ProfileProperty)
                .Include(s => s.FieldsOfStudy)
                .FirstOrDefaultAsync(m => m.ScholarshipId == scholarshipId);

            return scholarship;
        }

        public async Task<ScholarshipApplyViewModel> GetScholarshipApplicationViewModel(int applicationId)
        {
            var application = await _context.Application.FirstOrDefaultAsync(app => app.ApplicationId == applicationId);

            if (application == null)
                return null;

            var scholarship = await GetScholarship(application.ScholarshipId);

            if (scholarship == null)
                return null;

            var profile = await _context.Profile.Include(p => p.Guardians)
                .Include(p => p.FieldOfStudy)
                .FirstOrDefaultAsync(p => p.ProfileId == application.ProfileId);
            application.Profile = profile;

            var qset = await GetQuestionSetWithAnswers(application.AnswerGroupId);

            ScholarshipApplyViewModel vm = new ScholarshipApplyViewModel
            {
                Scholarship = scholarship,
                Application = application,
                QuestionSet = qset
            };

            return vm;
        }

        public async Task<Application> GetApplication (int scholarshipId, int profileId, int questionSetId)
        {
            var application = await _context.Application.FirstOrDefaultAsync(app => app.ScholarshipId == scholarshipId && app.ProfileId == profileId);

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

        private static string ProfileFieldToURI (string fieldname)
        {
            return "/Profile/Edit";
        }
    }
}
