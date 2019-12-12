﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    // Used to group AnswerSets - Any student response to a QuestionSet is an AnswerGroup
    public class AnswerGroup
    {
        [Key]
        public int AnswerGroupId { get; set; }

        public List<AnswerGroupSets> AnswerSets { get; set; }
    }
}
