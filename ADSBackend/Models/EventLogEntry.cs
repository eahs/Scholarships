using Scholarships.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Models
{
    public enum EventType
    {
        // General events
        LoggedIn = 1,
        UpdatedProfile = 2,

        // Scholarship-specific events
        ScholarshipView = 50,
        ScholarshipApplicationStarted = 51,
        ScholarshipApplicationCompleted = 52
    };

    // Used for general app user analytics
    public class EventLogEntry
    {
        [Key]
        public int EntryId { get; set; }
        public EventType Type { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int? Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Metadata { get; set; }  // JSON-encoded meta data
    }
}
