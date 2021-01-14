using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models
{
    public class FieldOfStudy
    {
        [Key]
        public int FieldOfStudyId { get; set; }

        public string Name { get; set; }

        public List<ScholarshipFieldOfStudy> Scholarships { get; set; }

    }
}
