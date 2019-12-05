using System;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models
{
    public class ScholarshipFieldOfStudy
    {
        public int ScholarshipId { get; set; }
        public Scholarship Scholarship { get; set; }

        public int FieldOfStudyId { get; set; }
        public FieldOfStudy FieldOfStudy { get; set; }
    }
}
