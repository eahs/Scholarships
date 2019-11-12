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
        public int FullName { get; set; }
        public int Relationship { get; set; }  // 0 = father, 1 = mother, 2 = guardian
        public int EmploymentStatus { get; set; }
        public string Occupation { get; set; }
        public string Employer { get; set; }
        public double AnnualIncome { get; set; }
    }
}
