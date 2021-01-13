using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Scholarships.Models.ScholarshipViewModels
{
    public class ScholarshipWinnersViewModel
    {
        public List<Scholarships.Models.Application> Applications { get; set; }
        public bool IsFiltered { get; set; } = false;
        [DisplayName("Scholarship Name Contains")]
        public string FilterName { get; set; } = "";
     }
}
