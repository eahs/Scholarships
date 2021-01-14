using Microsoft.AspNetCore.Mvc;

namespace Scholarships.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly Services.Configuration _configuration;
        private readonly Services.Cache _cache;

        public ApiController(Services.Configuration configuration, Services.Cache cache)
        {
            _configuration = configuration;
            _cache = cache;
        }

    }
}