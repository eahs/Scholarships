using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; } = true;

        public List<ScholarshipCategory> Scholarships { get; set; }
    }
}
