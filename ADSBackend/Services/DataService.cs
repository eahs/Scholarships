using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp.Extensions;
using Scholarships.Data;
using Scholarships.Models;
using Scholarships.Models.Forms;
using Scholarships.Models.Identity;
using Scholarships.Models.ScholarshipViewModels;
using Scholarships.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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

        /// <summary>
        /// Gets a specific profile by id or the profile of the current logged in user
        /// </summary>
        /// <param name="profileId">Profile Id of user to look up</param>
        /// <returns></returns>
        public async Task<Profile> GetProfileAsync(int profileId = -1)
        {
            Profile profile = null;

            if (profileId != -1)
            {
                profile = await _context.Profile.Include(p => p.Guardians)
                    .Include(p => p.FieldOfStudy)
                    .FirstOrDefaultAsync(p => p.ProfileId == profileId);

            }
            else
            {
                var user = await _userManager.GetUserAsync(_httpcontext.User);

                if (user == null)
                    return null;

                profile = await _context.Profile.Include(p => p.Guardians)
                    .Include(p => p.FieldOfStudy)
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                // If a junior logs in they will prematurely create a profile
                if (profile == null)
                {
                    // Let's see if there is one based off of email address
                    var iprofile = await _context.ImportedProfile.FirstOrDefaultAsync(p => p.Email == user.Email);

                    if (iprofile != null)
                    {
                        // Claim this profile
                        // The net effect of this code is that we serialize the imported profile and deserialize it to a standard profile.
                        // This process ensures that if we are to import new fields from a student data dump we need only to add the field to ImportedProfile and Profile
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

            var guardprop = scholarship.ProfileProperties.FirstOrDefault(gprop => gprop.ProfileProperty.PropertyKey == "Guardians");

            if (guardprop != null)
            {
                if (profile.Guardians.Count == 0)
                {
                    status.Add(new ScholarshipFieldStatus
                    {
                        FieldName = guardprop.ProfileProperty.PropertyName,
                        ErrorMessage = "You must have at least one family member listed in the Family Income section of your profile",
                        Validated = false,
                        FormURI = ProfileFieldToURI("Guardians")
                    });

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

        /// <summary>
        /// Retrieves a question set with all the answers
        /// </summary>
        /// <param name="AnswerGroupId"></param>
        /// <param name="BypassProfileVerification"></param>
        /// <returns></returns>
        public async Task<QuestionSet> GetQuestionSetWithAnswers(int AnswerGroupId, bool bypassProfileVerification = false)
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

            // If we bypass profile verification then we can just load the answer set as it is
            int profileId = bypassProfileVerification ? firstSet.ProfileId : -1;

            Profile profile = await GetProfileAsync(profileId);

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
                        if (answer.Config == null) continue;

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

        public async Task<Scholarship> GetScholarship(int scholarshipId)
        {
            var scholarship = await _context.Scholarship
                .Include(s => s.QuestionSet)
                .Include(s => s.ProfileProperties).ThenInclude(s => s.ProfileProperty)
                .Include(s => s.FieldsOfStudy)
                .Include(s => s.Categories)
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

        public async Task<Application> GetApplication(int scholarshipId, int profileId, int questionSetId)
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

        public async Task<List<Scholarship>> GetScholarshipUpdates(int count = 10)
        {
            var scholarships = await _context.Scholarship
                .Select(s => new Scholarship
                {
                    ScholarshipId = s.ScholarshipId,
                    Name = s.Name,
                    ReleaseDate = s.ReleaseDate,
                    DueDate = s.DueDate,
                    Published = s.Published,
                    Description = s.Description.StripHtml().HtmlDecode().CropWholeWords(150, null)
                })
                .Where(s => s.Published && s.DueDate >= DateTime.Now.AddDays(-7) && s.ReleaseDate <= DateTime.Now)
                .OrderByDescending(s => s.ReleaseDate)
                .Take(count)
                .ToListAsync();

            return scholarships ?? new List<Scholarship>();
        }

        public async Task<List<Scholarship>> GetScholarshipDeadlines(int count = 10)
        {
            var scholarships = await _context.Scholarship
                .Select(s => new Scholarship
                {
                    ScholarshipId = s.ScholarshipId,
                    Name = s.Name,
                    ReleaseDate = s.ReleaseDate,
                    Published = s.Published,
                    DueDate = s.DueDate
                })
                .Where(s => s.Published == true && s.DueDate >= DateTime.Now.AddDays(-7) && s.ReleaseDate <= DateTime.Now)
                .OrderBy(s => s.DueDate)
                .Take(count)
                .ToListAsync();

            return scholarships ?? new List<Scholarship>();
        }

        public async Task<List<Scholarship>> GetMyFavorites(int profileId, int count = 10)
        {
            var favorites = await _context.ScholarshipFavorite.Where(f => f.ProfileId == profileId)
                                                                      .Select(f => f.ScholarshipId)
                                                                      .ToListAsync();

            var scholarships = await _context.Scholarship
                .Select(s => new Scholarship
                {
                    ScholarshipId = s.ScholarshipId,
                    Name = s.Name,
                    ReleaseDate = s.ReleaseDate,
                    DueDate = s.DueDate,
                    Published = s.Published,
                    IsFavorite = true
                })
                .Where(s => s.Published && favorites.Contains(s.ScholarshipId))
                .OrderByDescending(s => s.DueDate)
                .Take(count)
                .ToListAsync();

            return scholarships ?? new List<Scholarship>();
        }

        public async Task<List<Application>> GetMyApplications(int profileId)
        {
            var apps = await _context.Application
                .Include(app => app.Scholarship)
                .Where(app => app.ProfileId == profileId)
                .Select(s => new Application
                {
                    Scholarship = new Scholarship
                    {
                        Name = s.Scholarship.Name,
                        ScholarshipId = s.ScholarshipId,
                        ReleaseDate = s.Scholarship.ReleaseDate,
                        DueDate = s.Scholarship.DueDate,
                        Published = s.Scholarship.Published
                    },
                    SubmittedDate = s.SubmittedDate,
                    Submitted = s.Submitted
                })
                .OrderByDescending(s => s.SubmittedDate)
                .ToListAsync();

            return apps ?? new List<Application>();
        }

        public async Task<Scholarship> DuplicateScholarshipAsync(int scholarshipId)
        {
            Scholarship scholarship = await _context.Scholarship.Include(s => s.FieldsOfStudy)
                                                                 .ThenInclude(fos => fos.FieldOfStudy)
                                                                 .Include(p => p.ProfileProperties).ThenInclude(prop => prop.ProfileProperty)
                                                                 .Include(p => p.Categories)
                                                                 .FirstOrDefaultAsync(s => s.ScholarshipId == scholarshipId);

            if (scholarship == null)
                return null;

            QuestionSet qset = await GetQuestionSet(scholarship.QuestionSetId);

            // First duplicate the existing question set
            QuestionSet _qset = new QuestionSet
            {
                Name = qset.Name ?? "",
                Description = qset.Description ?? "",
                SingularName = qset.SingularName ?? "",
                PluralName = qset.PluralName ?? "",
                AllowMultiple = qset.AllowMultiple,
                Questions = qset.Questions.Select(q => new Question
                {
                    Type = q.Type,
                    Order = q.Order,
                    Name = q.Name,
                    Description = q.Description,
                    Config = q.Config,
                    Required = q.Required,
                    ErrorMessage = q.ErrorMessage,
                    Options = q.Options.Select(opt => new QuestionOption
                    {
                        Order = opt.Order,
                        Name = opt.Name
                    }).ToList()
                }).ToList()
            };

            _context.Add(_qset);
            await _context.SaveChangesAsync();


            int schoolYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            if (currentMonth > 7)
                schoolYear++;

            DateTime newReleaseDate = DateTime.IsLeapYear(scholarship.ReleaseDate.Year) ? new DateTime(schoolYear, scholarship.ReleaseDate.Month, scholarship.ReleaseDate.Day == 29 && scholarship.ReleaseDate.Month == 2 ? 28 : scholarship.ReleaseDate.Day) :
                                                                                          new DateTime(schoolYear, scholarship.ReleaseDate.Month, scholarship.ReleaseDate.Day);

            DateTime newDueDate = DateTime.IsLeapYear(scholarship.DueDate.Year) ? new DateTime(schoolYear, scholarship.DueDate.Month, scholarship.DueDate.Day == 29 && scholarship.DueDate.Month == 2 ? 28 : scholarship.DueDate.Day) :
                                                                                  new DateTime(schoolYear, scholarship.DueDate.Month, scholarship.DueDate.Day);


            Scholarship _scholarship = new Scholarship
            {
                QuestionSetId = _qset.QuestionSetId,
                SponsorCompany = scholarship.SponsorCompany ?? "",
                SponsorName = scholarship.SponsorName ?? "",
                SponsorAddress1 = scholarship.SponsorAddress1 ?? "",
                SponsorAddress2 = scholarship.SponsorAddress2 ?? "",
                SponsorPhone = scholarship.SponsorPhone ?? "",
                SponsorEmail = scholarship.SponsorEmail ?? "",
                Name = scholarship.Name + " (Copy)" ?? "Duplicated Scholarship",
                Description = scholarship.Description ?? "",
                Eligibility = scholarship.Eligibility ?? "",
                Standards = scholarship.Standards ?? "",
                Amount = scholarship.Amount ?? "",
                ApplicationInstructions = scholarship.ApplicationInstructions ?? "",
                ApplyOnline = scholarship.ApplyOnline,
                TranscriptsRequired = scholarship.TranscriptsRequired,
                ReleaseDate = newReleaseDate,
                DueDate = newDueDate,
                DistrictMaintained = scholarship.DistrictMaintained,
                NumberOfYears = scholarship.NumberOfYears,
                IncomeVerificationRequired = scholarship.IncomeVerificationRequired,
                Published = false
            };


            _context.Scholarship.Add(_scholarship);

            await _context.SaveChangesAsync();

            // Now that the scholarship is duplicated, let's add back in all the categories, etc. it had
            if (scholarship.FieldsOfStudy?.Count > 0)
            {
                // Add new fields of study
                var newFieldsOfStudy = scholarship.FieldsOfStudy.Select(x => new ScholarshipFieldOfStudy
                {
                    ScholarshipId = _scholarship.ScholarshipId,
                    FieldOfStudyId = x.FieldOfStudyId
                });
                if (newFieldsOfStudy != null)
                    _context.ScholarshipFieldOfStudy.AddRange(newFieldsOfStudy);

                await _context.SaveChangesAsync();
            }

            if (scholarship.ProfileProperties?.Count > 0)
            {
                // Add new profile properties
                var newProfileProperties = scholarship.ProfileProperties.Select(x => new ScholarshipProfileProperty
                {
                    ScholarshipId = _scholarship.ScholarshipId,
                    ProfilePropertyId = x.ProfilePropertyId
                });
                if (newProfileProperties != null)
                    _context.ScholarshipProfileProperty.AddRange(newProfileProperties);

                await _context.SaveChangesAsync();
            }

            if (scholarship.Categories?.Count > 0)
            {
                // Add new categories
                var newCategories = scholarship.Categories.Select(x => new ScholarshipCategory
                {
                    ScholarshipId = _scholarship.ScholarshipId,
                    CategoryId = x.CategoryId
                });
                if (newCategories != null)
                    _context.ScholarshipCategory.AddRange(newCategories);
                await _context.SaveChangesAsync();

            }

            return _scholarship;
        }

        public async Task<ScholarshipWinnersViewModel> GetScholarshipWinnersAsync()
        {
            var winners = await _context.Application.Include(app => app.Scholarship)
                                                    .Include(app => app.Profile)
                                                    .Where(app => app.ApplicantAwarded)
                                                    .OrderByDescending(app => app.Scholarship.DueDate.Year)
                                                    .ThenBy(app => app.Scholarship.Name)
                                                    .ThenBy(app => app.Profile.LastName)
                                                    .ThenBy(app => app.Profile.FirstName)
                                                    .ToListAsync();

            return new ScholarshipWinnersViewModel
            {
                Applications = winners
            };
        }

        private static string ProfileFieldToURI(string fieldname)
        {
            return "/Profile/Edit";
        }
    }
}
