using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models
{
    public class Scholarship
    {
        [Key]
        public int ScholarshipId { get; set; }

        public string SponsorCompany { get; set; }
        public string SponsorName { get; set; }
        public string SponsorAddress1 { get; set; }
        public string SponsorAddress2 { get; set; }
        public string SponsorPhone { get; set; }
        public string SponsorEmail { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Eligibility { get; set; }
        public string Standards { get; set; }
        public string Amount { get; set; }
        public string ApplicationInstructions { get; set; }
        public bool ApplyOnline { get; set; }
        public bool TranscriptsRequired { get; set; }

        public DateTime ReleaseDate { get; set; }   // Date when scholarship becomes available
        public DateTime DueDate { get; set; } // Date when scholarship is due

        public List<ScholarshipCategory> Categories { get; set; }
        
    }
}
