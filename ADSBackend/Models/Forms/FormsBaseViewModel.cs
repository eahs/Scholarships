using Microsoft.AspNetCore.Mvc.ModelBinding;
using Scholarships.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public class FormsBaseViewModel
    {
        protected QuestionSetError _error = QuestionSetError.NoError;

        public string StatusMessage { get { return _error.GetDescription(); } }
        public bool IsError { get { return _error != QuestionSetError.NoError; } }
        public QuestionSetError ErrorCode
        {
            get { return _error; }
            set { _error = value; }
        }

        public List<ModelStateEntry> ModelStateErrors { get; set; }

    }
}
