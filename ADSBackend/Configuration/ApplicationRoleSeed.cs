using Scholarships.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Scholarships.Configuration
{
    public class ApplicationRoleSeed
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ApplicationRoleSeed(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public void CreateRoles()
        {
            var roles = new List<string>
            {
                "Admin",
                "User",
                "Student"
            };

            foreach (var roleName in roles)
            {
                if (!_roleManager.RoleExistsAsync(roleName).Result)
                {
                    var role = new ApplicationRole { Name = roleName };

                    _roleManager.CreateAsync(role).Wait();
                }
            }
        }
    }
}
