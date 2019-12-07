using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models
{
    public class ProfileProperty
    {
        [Key]
        public int ProfilePropertyId { get; set; }

        public string PropertyName { get; set; }
        public string PropertyKey { get; set; }

        public List<ScholarshipProfileProperty> Scholarships { get; set; }
    }
}
