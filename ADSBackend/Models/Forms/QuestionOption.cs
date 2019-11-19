﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public class QuestionOption
    {
        [Key]
        public int QuestionOptionId { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int Order { get; set; } = 0;

        public string Name { get; set; }

        [NotMapped]
        public bool Selected { get; set; }  // Used for rendering partial views
    }
}
