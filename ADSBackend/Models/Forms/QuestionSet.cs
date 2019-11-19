using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public class QuestionSet
    {
        [Key]
        public int QuestionSetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; }
        public List<AnswerSet> AnswerSets { get; set; }
        public string SingularName { get; set; } // e.g. "Activity", singular name for data being collected
        public string PluralName { get; set; }  // e.g. "Activities", plural name of data being collected
        public bool AllowMultiple { get; set; }  // Whether or not users can save more than one answerset for this questionset
    }
}
