using System.Collections.Generic;

namespace Scholarships.Models.Forms
{
    public class QuestionSetViewModel : FormsBaseViewModel
    {
        public QuestionSet QuestionSet { get; set; }
        public List<QuestionViewModel> Questions { get; set; }

        public string PrimaryForm { get; set; }
        public string FormId { get; set; }
    }
}
