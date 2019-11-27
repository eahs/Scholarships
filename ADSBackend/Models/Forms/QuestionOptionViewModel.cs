using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public class QuestionOptionViewModel : FormsBaseViewModel
    {
        public int QuestionIndex { get; set; }
        public int Index { get; set; }   // Used for forms
        public Question Question { get; set; }
        public QuestionOption QuestionOption { get; set; }
        public string QuestionOptionForm { get; set; }
    }
}
