﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scholarships.Models.Forms
{
    public class QuestionSet
    {
        [Key]
        public int QuestionSetId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; }
        [Required]
        [DisplayName("Singular Name")]
        public string SingularName { get; set; } = "Item"; // e.g. "Activity", singular name for data being collected
        [Required]
        [DisplayName("Plural Name")]
        public string PluralName { get; set; } = "Items";  // e.g. "Activities", plural name of data being collected

        [DisplayName("Allow Multiple Entries")]
        public bool AllowMultiple { get; set; } = false; // Whether or not users can save more than one answerset for this questionset

        [NotMapped]
        public List<AnswerSet> AnswerSets { get; set; }

        [NotMapped]
        public int AnswerGroupId { get; set; }  // Only used by scholarship application form to provide info to generate form correctly
    }
}
