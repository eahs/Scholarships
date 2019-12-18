using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }
        public int AnswerSetId { get; set; }
        public AnswerSet AnswerSet { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public string Response { get; set; }  // For free response, stores answer as a string
        public int QuestionOptionId { get; set; }  // For multiple choice, stores answer as the option id
        public DateTime DateTime { get; set; } = DateTime.Now;  // For datetime, stores answer as a Date
        public string Config { get; set; }  // Any extraneous JSON-encoded data for this answer

        [NotMapped]
        public List<int> QuestionOptions { get; set; }

    }
}
