using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models.Forms
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        public string Response { get; set; }  
        public string Config { get; set; }  // Any extraneous JSON-encoded data for this answer
    }
}
