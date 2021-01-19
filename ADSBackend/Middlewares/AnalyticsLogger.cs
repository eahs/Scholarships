using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Scholarships.Data;
using Scholarships.Models.Identity;

namespace Scholarships.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AnalyticsLogger
    {
        private readonly RequestDelegate _next;

        public AnalyticsLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var controllerActionDescriptor = httpContext
                      .GetEndpoint()
                      .Metadata
                      .GetMetadata<ControllerActionDescriptor>();

                var controllerName = controllerActionDescriptor.ControllerName;
                var actionName = controllerActionDescriptor.ActionName;
                var Id = controllerActionDescriptor.Id;

                var userId = userManager.GetUserId(httpContext.User); // httpContext.User.FindFirst(ClaimTypes.NameIdentifier)
            }

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AnalyticsLoggerExtensions
    {
        public static IApplicationBuilder UseAnalyticsLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AnalyticsLogger>();
        }
    }
}
