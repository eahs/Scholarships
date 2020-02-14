using Scholarships.Data;
using Scholarships.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Scholarships.Models.ScholarshipViewModels;
using Scholarships.Models.Forms;

namespace Scholarships.Services
{
    public interface IDataService
    {
        // Profile related
        Task<Profile> GetProfileAsync(int profileId = -1);
        Task IncludeFavorites(int profileId, List<Scholarship> scholarships);
        Task IncludeApplications(int profileId, List<Scholarship> scholarships);
        Task<List<ScholarshipFieldStatus>> VerifyApplicationStatus(Scholarship scholarship);
        Task<QuestionSet> GetQuestionSet(int QuestionSetId);
        Task<QuestionSet> GetQuestionSetWithAnswers(int AnswerGroupId, bool bypassProfileVerification = false);
        Task<Scholarship> GetScholarship(int scholarshipId);
        Task<ScholarshipApplyViewModel> GetScholarshipApplicationViewModel(int applicationId);
        Task<Application> GetApplication(int scholarshipId, int profileId, int questionSetId);
        Task<List<Scholarship>> GetScholarshipUpdates(int count = 10);
        Task<List<Scholarship>> GetScholarshipDeadlines(int count = 10);
        Task<List<Scholarship>> GetMyFavorites(int profileId, int count = 10);
        Task<List<Application>> GetMyApplications(int profileId);

        // Model validation tasks
        void ScrubModelState(string bindingFields, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary mState);

    }
}
