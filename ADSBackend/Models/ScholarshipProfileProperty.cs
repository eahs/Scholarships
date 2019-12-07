using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models
{
    public class ScholarshipProfileProperty
    {
        public int ScholarshipId { get; set; }
        public Scholarship Scholarship { get; set; }
        public int ProfilePropertyId { get; set; }
        public ProfileProperty ProfileProperty { get; set; }
    }
}
