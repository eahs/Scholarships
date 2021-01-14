using System.ComponentModel.DataAnnotations;

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
