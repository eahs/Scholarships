using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("analytics")]
        public async Task<object> GetDailyAnalytics(string uri)
        {
            List<object> adata = new List<object>();

            var stats = await _context.EventLogDaily.Where(eld => uri == (eld.Controller+"/"+eld.Action+(eld.Id == null ? "" : "/" +eld.Id)))
                .OrderBy(eld => eld.Date)
                .ToArrayAsync();

            foreach (var stat in stats)
            {
                adata.Add(new
                {
                    x = stat.Date.ToString("MM/dd/yyyy"),
                    y = stat.Count
                });
            }

            return new
            {
                data = new
                {
                    datasets = new object[]
                    {
                        new {
                            label = "Scholarship Views",
                            data = adata
                        }
                    }
                }
                
            };
        }
    }
}