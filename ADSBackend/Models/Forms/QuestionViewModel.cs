﻿namespace Scholarships.Models.Forms
{
    public class QuestionViewModel : FormsBaseViewModel
    {
        public int Index { get; set; }   // Used for forms
        public Question Question { get; set; }
        public string QuestionForm { get; set; }
    }
}
