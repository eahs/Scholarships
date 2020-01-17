using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models
{
    public class ScholarshipFavorite
    {
        public int ScholarshipId { get; set; }

        public Scholarship Scholarship { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
    }
}
