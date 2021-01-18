using System.Collections.Generic;
using Scholarships.Models.Forms;

namespace Scholarships.Models.ScholarshipViewModels
{
    public class ScholarshipApplyViewModel
    {
        public Scholarship Scholarship { get; set; }
        public Application Application { get; set; }
        public QuestionSet QuestionSet { get; set; }
        public List<Scholarship> ScholarshipsWon { get; set; }
    }
}
