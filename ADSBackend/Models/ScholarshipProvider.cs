using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Scholarships.Models.Identity;

namespace Scholarships.Models
{
    public class ScholarshipProvider
    {
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int ScholarshipId { get; set; }
        public Scholarship Scholarship { get; set; }
    }
}
