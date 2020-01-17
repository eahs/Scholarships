using Scholarships.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly Services.Configuration Configuration;
        private readonly Services.Cache _cache;

        public ApiController(Services.Configuration configuration, Services.Cache cache)
        {
            Configuration = configuration;
            _cache = cache;
        }

    }
}