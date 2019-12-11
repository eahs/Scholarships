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
        Task<Profile> GetProfileAsync();
        Task<List<ScholarshipFieldStatus>> VerifyApplicationStatus(Scholarship scholarship);

        // Model validation tasks
        void ScrubModelState(string bindingFields, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary mState);

        // Question set tasks
        Task<QuestionSet> GetQuestionSet(int QuestionSetId);
    }
}
