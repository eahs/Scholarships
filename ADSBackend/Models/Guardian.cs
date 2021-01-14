using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models
{
    public class Guardian
    {
        [Key]
        public int GuardianId { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }

        [Required]
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [DisplayName("Relationship to Scholarship Applicant")]
        [Range(1, 4, ErrorMessage = "You must provide the relationship this individual has to the scholarship applicant")]
        public int Relationship { get; set; }  // 0 = self, 1 = father, 2 = mother, 3 = guardian
        [DisplayName("Employment Status")]
        [Range(1, 3, ErrorMessage = "You must provide the employment status for this individual")]
        public int EmploymentStatus { get; set; }
        public string Occupation { get; set; }
        public string Employer { get; set; }
        [DisplayName("Annual Income")]
        public double AnnualIncome { get; set; }
    }
}
