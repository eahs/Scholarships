using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Scholarships.Models.ScholarshipViewModels
{
    public class ScholarshipListViewModel
    {
        public List<Scholarships.Models.Scholarship> Scholarships { get; set; }
        public bool IsFiltered { get; set; } = false;
        [DisplayName("Scholarship Name Contains")]
        public string FilterName { get; set; } = "";
        public MultiSelectList FilterFieldsOfStudy { get; set; }
        public MultiSelectList FilterCategory { get; set; }
        public List<int> FieldsOfStudyIds { get; set; }
        public List<int> CategoryIds { get; set; }
        public bool LocalOnly { get; set; } = false;
    }
}
