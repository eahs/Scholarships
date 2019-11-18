using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public class QuestionOption
    {
        [Key]
        public int QuestionOptionId { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public string Name { get; set; }
    }
}
