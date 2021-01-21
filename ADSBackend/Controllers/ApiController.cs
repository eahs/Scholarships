using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scholarships.Data;
using Scholarships.Models;

namespace Scholarships.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly Services.Configuration _configuration;
        private readonly Services.Cache _cache;
        private readonly ApplicationDbContext _context;

        public ApiController(Services.Configuration configuration, Services.Cache cache, ApplicationDbContext context)
        {
            _configuration = configuration;
            _cache = cache;
            _context = context;
        }

        [Authorize(Roles = "Admin,Manager,Provider")]
        [HttpGet("analytics/sum")]
        public async Task<object> GetSummaryAnalytics()
        {

            var stats = await _context.EventLogDaily.Where(eld => eld.Date > DateTime.Now.AddDays(-90))
                                                    .GroupBy(eld => eld.Date)
                                                    .Select(g => new EventLogDaily
                                                    {
                                                        Date = g.Key,
                                                        Count = g.Sum(s => s.Count)
                                                    })
                                                    .OrderBy(eld => eld.Date)
                                                    .ToListAsync();


            var adata = await ChartifyMatchingLogs(stats);

            return new
            {
                data = new
                {
                    datasets = new object[]
                    {
                        new {
                            borderColor = "#1b39c2",
                            fill = false,
                            lineTension = 0,
                            label = "Daily Traffic Summary",
                            data = adata
                        }
                    }
                }

            };
        }


        [Authorize(Roles = "Admin,Manager,Provider")]
        [HttpGet("analytics")]
        public async Task<object> GetDailyAnalytics(string? label, string uri)
        {
            uri = uri.TrimEnd('/');
            label ??= uri;

            var stats = await _context.EventLogDaily.Where(eld => uri == (eld.Controller + "/" + eld.Action + (eld.Id == null ? "" : "/" + eld.Id)))
                                                    .OrderBy(eld => eld.Date)
                                                    .ToListAsync();


            var adata = await ChartifyMatchingLogs(stats);

            return new
            {
                data = new
                {
                    datasets = new object[]
                    {
                        new {
                            borderColor = "#1b39c2",
                            fill = false,
                            lineTension = 0,
                            label,
                            data = adata
                        }
                    }
                }
                
            };
        }

        /// <summary>
        /// Converts logs into a dataset for chart.js
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        private async Task<List<object>> ChartifyMatchingLogs (List<EventLogDaily> stats)
        {
            List<object> adata = new List<object>();

            foreach (var stat in stats)
            {
                adata.Add(new
                {
                    x = stat.Date.ToString("MM/dd/yyyy"),
                    y = stat.Count
                });
            }

            return adata;
        }
    }
}