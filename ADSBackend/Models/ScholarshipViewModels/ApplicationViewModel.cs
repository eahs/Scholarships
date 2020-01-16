using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.ScholarshipViewModels
{
    public class ApplicationViewModel
    {
        public int ScholarshipId { get; set; }
        public Scholarship Scholarship { get; set; }
        public List<Application> Applications { get; set; }
        

    }
}
