using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public class AnswerOption
    {
        [Key]
        public int AnswerOptionId { get; set; }

        public int AnswerId { get; set; }
        public Answer Answer { get; set; }

        public int QuestionOptionId { get; set; }    
        public QuestionOption Option { get; set; }
    }
}
