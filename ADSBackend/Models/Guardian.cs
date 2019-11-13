using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models
{
    public class Guardian
    {
        [Key]
        public int GuardianId { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        public string FullName { get; set; }
        public int Relationship { get; set; }  // 0 = self, 1 = father, 2 = mother, 3 = guardian
        public int EmploymentStatus { get; set; }
        public string Occupation { get; set; }
        public string Employer { get; set; }
        public double AnnualIncome { get; set; }
    }
}
