﻿using Microsoft.AspNetCore.Identity;
using Scholarships.Data;
using Scholarships.Models.Identity;
using System;
using System.Linq;

namespace Scholarships.Configuration
{
    public class ApplicationUserSeed
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ApplicationUserSeed(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public void CreateAdminUser()
        {
            ApplicationUser user = _userManager.FindByNameAsync("admin").Result;
            if (user != null)
            {
                //var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                //_userManager.ResetPasswordAsync(user, token, "NewPassword").Wait();

                /*
                if (user.Email == null || user.Email == "")
                    user.Email = "tanczosm@eastonsd.org";

                _userManager.UpdateAsync(user);

                var applicationUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);

                if (applicationUser != null)
                {
                    if (applicationUser.Email == null || applicationUser.Email == "")
                        applicationUser.Email = "tanczosm@eastonsd.org";

                    _context.Users.Update(applicationUser);
                }
                */

                return;
            }

            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                FirstName = "Admin",
                Email = "tanczosm@eastonsd.org"
            };

            IdentityResult result;
            try
            {
                result = _userManager.CreateAsync(adminUser, "Password123!").Result;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while creating the admin user: " + e.InnerException);
            }

            if (!result.Succeeded)
            {
                throw new Exception("The following error(s) occurred while creating the admin user: " + string.Join(" ", result.Errors.Select(e => e.Description)));
            }

            _userManager.AddToRoleAsync(adminUser, "Admin").Wait();
        }
    }
}
