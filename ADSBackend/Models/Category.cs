using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        public string Name { get; set; }
        public List<ScholarshipCategory> Scholarships { get; set; }
    }
}
