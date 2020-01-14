using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.ScholarshipViewModels
{
    public class ScholarshipDetailsViewModel
    {
        public Scholarship Scholarship { get; set; }
        public bool CanApply { get; set; } = false;
        public bool ApplicationCompleted { get; set; }  
        public bool ProfileCompleted { get; set; }   // True if user has filled out all the fields they need to fill out
        public List<ScholarshipFieldStatus> FieldStatus { get; set; }

    }
}
