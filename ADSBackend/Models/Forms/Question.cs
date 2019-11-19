using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public enum QuestionType
    {
        Checkboxes = 1,
        Dropdown = 2,
        MultipleChoice = 3,
        Date = 4,
        ShortAnswer = 5,
        Paragraph = 6,
        FileUpload = 7
    }

    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        public QuestionType Type { get; set; } = QuestionType.Checkboxes;
        public int Order { get; set; } = 0;
        public string Name { get; set; }
        public string Decription { get; set; }
        public string Config { get; set; }  // JSON configuration
        public bool Required { get; set; }
        public string ErrorMessage { get; set; }
        public List<QuestionOption> Options { get; set; }

    }
}
