using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Scholarships.Data;
using Scholarships.Models.Identity;
using Microsoft.AspNetCore.Routing;
using Scholarships.Models;

namespace Scholarships.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AnalyticsLogger
    {
        private readonly RequestDelegate _next;
        private string[] ControllersToTrack = { "Admin", "Scholarships", "Articles", "Home", "Profile", "Scholarships" };

        public AnalyticsLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            try
            {

                if (httpContext.User.Identity.IsAuthenticated)
                {
                    var controllerActionDescriptor = httpContext
                          .GetEndpoint()
                          .Metadata
                          .GetMetadata<ControllerActionDescriptor>();
                    
                    var controllerName = controllerActionDescriptor.ControllerName;

                    if (ControllersToTrack.Contains(controllerName))
                    {
                        var actionName = controllerActionDescriptor.ActionName;
                        var Id = httpContext.GetRouteValue("id");
                        var userId = userManager.GetUserId(httpContext.User);

                        try
                        {
                            int? cleanId = null;
                            int cleanUserId;

                            if (Id is string)
                            {
                                int temp;

                                if (int.TryParse(Id as string, out temp))
                                {
                                    cleanId = temp;
                                }
                            }


                            bool success = int.TryParse(userId, out cleanUserId);

                            if (success)
                            {
                                context.EventLogEntry.Add(new EventLogEntry
                                {
                                    UserId = cleanUserId,
                                    Controller = controllerName,
                                    Action = actionName,
                                    Id = cleanId,
                                    DateTime = DateTime.Now,
                                    Metadata = "{}"
                                });
                                await context.SaveChangesAsync();
                            }


                        }
                        catch (Exception)
                        {
                            // Ignore
                        }
                    }
                }

            }
            catch (Exception)
            {
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
