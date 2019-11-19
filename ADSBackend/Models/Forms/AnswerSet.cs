using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public class AnswerSet
    {
        [Key]
        public int AnswerSetId { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        public int QuestionSetId { get; set; }
        public QuestionSet QuestionSet { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
