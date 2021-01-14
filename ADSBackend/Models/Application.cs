using Scholarships.Models.Forms;
using System;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models
{
    public class Application
    {
        [Key]
        public int ApplicationId { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        public int ScholarshipId { get; set; }
        public Scholarship Scholarship { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime SubmittedDate { get; set; } = DateTime.Now;
        public int SubmissionStage { get; set; } = 1;  // 1 = In-Progress, 2 = Submitted
        public bool Submitted { get; set; } = false;
        public string Signature { get; set; }
        public bool AcceptRelease { get; set; } = false;
        public bool FerpaTerms { get; set; } = false;  // Does student waive rights to see confidential information contained in scholarship submission (like recommendation letters)
        public DateTime SignatureDate { get; set; }
        public AnswerGroup AnswerGroup { get; set; }
        public int AnswerGroupId { get; set; }
        public string ProfileSnapshot { get; set; }  // JSON key/value store of user's profile at the time of submission

        public int ApplicantScore { get; set; } = 0;  // Score for applicant determined during evaluation process
        public bool ApplicantAwarded { get; set; } = false;  // This applicant was awarded the scholarship
        public DateTime ApplicantAwardDate { get; set; }  // Date applicant was awarded scholarship
        public bool ApplicantFavorite { get; set; }   // This applicant is a top applicant
        public string ApplicantNotes { get; set; }  // Any notes about the applicant
    }
}
