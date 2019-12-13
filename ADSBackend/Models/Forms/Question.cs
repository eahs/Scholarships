using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    // Note that any new QuestionTypes or changes made to these enumerated types will need 
    // to be accompanied by a corresponding similarly-named partial view in the AnswerGroup View folder
    public enum QuestionType
    {
        Checkboxes = 1,
        Dropdown = 2,
        MultipleChoice = 3,
        Date = 4,
        Time = 5,
        ShortAnswer = 6,
        Paragraph = 7,
        FileUpload = 8
    }

    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        public int QuestionSetId { get; set; }
        public QuestionSet QuestionSet { get; set; }
        public QuestionType Type { get; set; } = QuestionType.Checkboxes;
        public int Order { get; set; } = 0;
        public string Name { get; set; }
        public string Description { get; set; }
        public string Config { get; set; }  // JSON configuration
        public bool Required { get; set; }
        public string ErrorMessage { get; set; }
        public List<QuestionOption> Options { get; set; }

        // These fields are only used when producing a form
        [NotMapped]
        public Answer Answer { get; set; }  

        [NotMapped]
        public int Index { get; set; } = 0;

        [NotMapped]
        public int AnswerSetIndex { get; set; } = 0;

    }
}
