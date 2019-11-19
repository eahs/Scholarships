﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public enum QuestionSetError
    {
        [Description("OK")]
        NoError = 0,

        [Description("You are not authorized to access this question set")]
        NotAuthorized = 1,  // User is not authorized to access this question set

        [Description("The selected question set was not found")]
        NotFound = 2,

        [Description("There are some errors with the submitted form that need fixing")]
        InvalidForm = 3
    }
}