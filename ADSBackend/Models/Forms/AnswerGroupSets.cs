using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public class AnswerGroupSets
    {
        public int AnswerGroupId { get; set; }
        public AnswerGroup AnswerGroup { get; set; }
        public int AnswerSetId { get; set; }
        public AnswerSet AnswerSet { get; set; }
    }
}
