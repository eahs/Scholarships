using System;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models
{
    public class ScholarshipFieldOfStudy
    {
        [Key]
        public int ScholarshipFieldOfStudyId { get; set; }

        public int ScholarshipId { get; set; }
        public Scholarship Scholarship { get; set; }

        public int FieldOfStudyId { get; set; }
        public FieldOfStudy FieldOfStudy { get; set; }
    }
}
