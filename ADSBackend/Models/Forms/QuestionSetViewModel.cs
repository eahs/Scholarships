using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Scholarships.Util;

namespace Scholarships.Models.Forms
{
    public class QuestionSetViewModel
    {
        private QuestionSetError _error = QuestionSetError.NoError;

        public string StatusMessage { get { return _error.GetDescription(); } }
        public bool IsError { get { return _error != QuestionSetError.NoError; } }
        public QuestionSetError ErrorCode { 
            get { return _error; }
            set { _error = value; } 
        }
        public QuestionSet QuestionSet { get; set; }
        public string PrimaryForm { get; set; }
        public string FormId { get; set; }
        public List<ModelStateEntry> ModelStateErrors { get; set; }
    }
}
