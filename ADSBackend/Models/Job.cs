using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models
{
    public class Job
    {
        [Key]
        public int JobId { get; set; }

        public string Name { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Started { get; set; } 
        public DateTime Ended { get; set; }
        public bool Completed { get; set; } = false;
        public string StatusMessage { get; set; } = "Currently Waiting";
        public string Type { get; set; } = "applications";
        public int ForeignKey { get; set; }  // id for related job, example could be a ScholarshipId
        public string Configuration { get; set; } = "{}";  // json-encoded configuration for job
    }
}
