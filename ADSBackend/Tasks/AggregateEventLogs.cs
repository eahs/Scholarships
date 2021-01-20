using Microsoft.EntityFrameworkCore;
using Scholarships.Data;
using Scholarships.Models;
using Scholarships.Util;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Tasks
{
    public class AggregateEventLogs : IAggregateEventLogs
    {
        private readonly ApplicationDbContext _context;

        public AggregateEventLogs (ApplicationDbContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            try
            {
                AsyncHelpers.RunSync(ProcessEventLogs);
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception thrown while trying to index event logs");
            }
        }

        public async Task ProcessEventLogs ()
        {
            var eventLogAggregationIndex = await _context.EventLogAggregationIndex.FirstOrDefaultAsync();

            if (eventLogAggregationIndex == null)
            {
                eventLogAggregationIndex = new Models.EventLogAggregationIndex
                {
                    LastDailyEventLogEntryId = -1
                };

                _context.EventLogAggregationIndex.Add(eventLogAggregationIndex);

                await _context.SaveChangesAsync();
            }

            var entries = await _context.EventLogEntry.Where(le => le.EntryId > eventLogAggregationIndex.LastDailyEventLogEntryId)
                                                      .ToListAsync();

            if (entries.Count > 0)
            {
                // Create a dictionary where each key is for one day, and for that day there is another dictionary that maps uri to stats about that uri
                Dictionary<DateTime, Dictionary<string, EventLogDaily>> stats = new Dictionary<DateTime, Dictionary<string, EventLogDaily>>();

                // Iterate through each log entry
                foreach (var entry in entries)
                {
                    // Check to see if we currently are tracking this particular day for any type of page
                    if (!stats.ContainsKey(entry.DateTime.Date))
                    {
                        stats[entry.DateTime.Date] = new Dictionary<string, EventLogDaily>();
                    }

                    var dayStats = stats[entry.DateTime.Date];

                    string uri = $"{entry.Controller}/{entry.Action}/{entry.Id}";

                    if (!dayStats.ContainsKey(uri))
                    {
                        EventLogDaily evd = new EventLogDaily
                        {
                            Controller = entry.Controller,
                            Action = entry.Action,
                            Id = entry.Id,
                            Count = 0
                        };
                        dayStats.Add(uri, evd);
                    }

                    dayStats[uri].Count++;
                }

                // Update the last indexed table
                eventLogAggregationIndex.LastDailyEventLogEntryId = entries.Max(e => e.EntryId);
                _context.EventLogAggregationIndex.Update(eventLogAggregationIndex);

                // Now update the eventlogdaily table
                foreach (DateTime day in stats.Keys)
                {
                    // Get all of the various unique page logs that were visited on day
                    var dayStats = stats[day].Values;

                    // Now record all the aggregated daily event logs for each page
                    foreach (EventLogDaily evd in dayStats)
                    {
                        var eventLogDaily = await _context.EventLogDaily.FirstOrDefaultAsync(eld => eld.Date == evd.Date && 
                                                                                                    eld.Controller == evd.Controller && 
                                                                                                    eld.Action == evd.Action && 
                                                                                                    eld.Id == evd.Id);

                        if (eventLogDaily == null)
                        {
                            _context.EventLogDaily.Add(evd);
                        }
                        else
                        {
                            eventLogDaily.Count += evd.Count;
                            _context.EventLogDaily.Update(eventLogDaily);
                        }
                    }
                }

                // Commit changes to database
                await _context.SaveChangesAsync();
            }
        }
    }
}
