﻿using Scholarships.Models.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scholarships.Models
{
    public class Scholarship
    {
        [Key]
        public int ScholarshipId { get; set; }

        [DisplayName("Published")]
        public bool Published { get; set; } = false;

        // Date when scholarship was originally created
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        [DisplayName("Sponsor Company")]
        public string SponsorCompany { get; set; }
        [Required]
        [DisplayName("Sponsor Name")]
        public string SponsorName { get; set; }
        [DisplayName("Sponsor Address (Line 1)")]
        public string SponsorAddress1 { get; set; }
        [DisplayName("Sponsor Address (Line 2)")]
        public string SponsorAddress2 { get; set; }

        [DataType(DataType.PhoneNumber)]
        [DisplayName("Sponsor Phone Number")]
        public string SponsorPhone { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Sponsor Contact Email")]
        public string SponsorEmail { get; set; }

        [Required]
        [DisplayName("Scholarship Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Eligibility { get; set; }
        public string Standards { get; set; }
        public string Amount { get; set; }
        [DisplayName("Application Instructions")]
        public string ApplicationInstructions { get; set; }

        [DisplayName("Apply Online?")]
        public bool ApplyOnline { get; set; }
        [DisplayName("Transcripts Required?")]
        public bool TranscriptsRequired { get; set; }
        [DisplayName("Requires Income Verification?")]
        public bool IncomeVerificationRequired { get; set; }

        [DisplayName("Maintained by District?")]
        public bool DistrictMaintained { get; set; } = false;

        [DisplayName("Number of Years For Scholarship")]
        [Range(1, 12, ErrorMessage = "This scholarship must be granted to recipient for at least one year but no more than 12")]
        public int NumberOfYears { get; set; } = 1;


        [DisplayName("Public Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }   // Date when scholarship becomes available
        [DisplayName("Scholarship Deadline")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } // Date when scholarship is due

        public int QuestionSetId { get; set; }
        public QuestionSet QuestionSet { get; set; }

        public List<ScholarshipCategory> Categories { get; set; }
        public List<ScholarshipFieldOfStudy> FieldsOfStudy { get; set; }
        public List<ScholarshipProfileProperty> ProfileProperties { get; set; }

        [NotMapped]
        public List<int> FieldsOfStudyIds { get; set; }

        [NotMapped]
        public List<int> ProfilePropertyIds { get; set; }

        [NotMapped]
        public List<int> CategoryIds { get; set; }

        [NotMapped]
        public bool IsFavorite { get; set; } = false; // Used for mapping whether a user considers this scholarship a favorite

        [NotMapped]
        public bool HasApplied { get; set; } = false; // Used for mapping whether a user has applied for this scholarship

        [NotMapped]
        public int ApplicantCount { get; set; } = 0; // Used for counting number of users who have applied

        [NotMapped]
        public int ApplicantPending { get; set; } = 0;  // Used for counting number of users who have started application process

        [NotMapped]
        public string NameAndYear => $"{Name} ({DueDate.Year})";
    }
}
