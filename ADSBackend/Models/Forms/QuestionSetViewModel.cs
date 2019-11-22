using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Scholarships.Util;

namespace Scholarships.Models.Forms
{
    public class QuestionSetViewModel : FormsBaseViewModel
    {
        public QuestionSet QuestionSet { get; set; }
        public string PrimaryForm { get; set; }
        public string FormId { get; set; }
    }
}
