using Scholarships.Models;
using Scholarships.Models.Forms;
using Scholarships.Models.ScholarshipViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

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
