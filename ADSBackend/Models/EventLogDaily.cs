using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models
{
    public class EventLogDaily
    {
        [Key]
        public int EventLogDailyId { get; set; }
        public DateTime Date { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int? Id { get; set; }
        public int Count { get; set; }
    }
}
