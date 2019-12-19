using Scholarships.Models.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models
{
    public class Application
    {
        [Key]
        public int ApplicationId { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        public int ScholarshipId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime SubmittedDate { get; set; } = DateTime.Now;
        public int SubmissionStage { get; set; } = 1;
        public bool Submitted { get; set; } = false;
        public AnswerGroup AnswerGroup { get; set; }
        public int AnswerGroupId { get; set; }
        public string ProfileSnapshot { get; set; }  // JSON key/value store of user's profile at the time of submission
    }
}
