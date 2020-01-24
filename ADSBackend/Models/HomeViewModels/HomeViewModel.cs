using Scholarships.Models.Identity;
using System.Collections.Generic;

namespace Scholarships.Models.HomeViewModels
{
    public class HomeViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Article> Articles { get; set; }

        // User dashboard properties
        public bool CompletedProfile { get; set; } = false;
        public bool CompletedScholarshipApplication { get; set; }

        public List<Scholarship> ScholarshipUpdates { get; set; }
        public List<Scholarship> ScholarshipsUpcomingDeadlines { get; set; }
        public List<Application> MyApplications { get; set; }
        public List<Scholarship> MyFavorites { get; set; }

        // Admin dashboard properties
        public List<Scholarship> InProgress { get; set; }
    }
}
