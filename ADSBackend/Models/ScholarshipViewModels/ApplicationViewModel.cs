using System.Collections.Generic;

namespace Scholarships.Models.ScholarshipViewModels
{
    public class ApplicationViewModel
    {
        public int ScholarshipId { get; set; }
        public Scholarship Scholarship { get; set; }
        public List<Application> Applications { get; set; }


    }
}
