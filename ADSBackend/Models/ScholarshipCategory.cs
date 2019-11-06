using System;
using System.ComponentModel.DataAnnotations;

namespace ADSBackend.Models
{
    public class ScholarshipCategory
    {
        [Key]
        public int ScholarshipCategoryId { get; set; }

        public int ScholarshipId { get; set; }
        public Scholarship Scholarship { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
