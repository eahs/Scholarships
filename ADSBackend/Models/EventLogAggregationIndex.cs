using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models
{
    public class EventLogAggregationIndex
    {
        [Key]
        public int AggId { get; set; }
        public int LastDailyEventLogEntryId { get; set; }  // Id of last EventLogEntry to be indexed
    }
}
