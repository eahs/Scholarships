using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scholarships.Data;
using Scholarships.Models;
using Scholarships.Models.Identity;
using Scholarships.Util;

namespace Scholarships.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Profiles
        public async Task<IActionResult> Index()
        {
            var profile = await GetProfileAsync();

            return View(profile);
        }

        public async Task<Profile> GetProfileAsync ()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return null;

            var profile = await _context.Profile.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (profile == null)
            {
                profile = new Profile
                {
                    UserId = user.Id,
                    Email = user.Email
                };
                _context.Profile.Add(profile);
                await _context.SaveChangesAsync();
            }

            return profile;
        }

        // GET: Profiles/Edit
        public async Task<IActionResult> Edit()
        {
            var profile = await GetProfileAsync();
            var genders = new[]
            {
                new { Name = "Male", GenderId = 0},
                new { Name = "Female", GenderId = 1},
                new { Name = "Other", GenderId = 2}
            }.ToList();
            var livingSituations = new[]
            {
                new { Name = "Live On Campus", Key = "oncampus"},
                new { Name = "Commute", Key = "commute"}
            }.ToList();

            ViewBag.Genders = new SelectList(genders, "GenderId", "Name");
            ViewBag.LivingSituations = new SelectList(livingSituations, "Key", "Name");
            ViewBag.States = FormHelper.States;

            return View(profile);
        }

        // POST: Profiles/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> EditProfile([Bind("ProfileId,FirstName,LastName,StudentId,BirthDate,Gender,Email,Address1,Address2,City,ZipCode,Phone")] Profile profile)
        {
            var _profile = await GetProfileAsync();

            if (_profile == null || _profile.ProfileId != profile.ProfileId)
            {
                return FormHelper.JsonStatus("NotFound");
            }

            // Copy values from profile into _profile
            _profile.FirstName = profile.FirstName;
            _profile.LastName = profile.LastName;
            _profile.StudentId = profile.StudentId;
            _profile.BirthDate = profile.BirthDate;
            _profile.Gender = profile.Gender;
            _profile.Email = profile.Email;
            _profile.Address1 = profile.Address1;
            _profile.Address2 = profile.Address2;
            _profile.City = profile.City;
            _profile.ZipCode = profile.ZipCode;
            _profile.Phone = profile.Phone;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(_profile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfileExists(profile.ProfileId))
                    {
                        return FormHelper.JsonStatus("NotFound"); ;
                    }
                    else
                    {
                        throw;
                    }
                }
                return FormHelper.JsonStatus("Success");
            }

            return FormHelper.JsonStatus("InvalidRequest"); ;
        }

        // POST: Profiles/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAcademic(int id, [Bind("ProfileId,ClassRank,GPA,SATScoreMath,SATScoreReading,ACTScore")] Profile profile)
        {
            var _profile = await GetProfileAsync();

            if (_profile == null || _profile.ProfileId != profile.ProfileId)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(_profile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfileExists(profile.ProfileId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        //         public async Task<IActionResult> EditAcademic(int id, [Bind("ProfileId,ClassRank,GPA,SATScoreMath,SATScoreReading,ACTScore,CollegeAttending,TuitionYearly,RoomBoard,TuitionTotal,CollegeAccepted,CollegeIntendedMajor,LivingSituation,OtherAid,ActivitiesSchool,ActivitiesCommunity,SchoolOffices,SpecialCircumstances,FatherName,FatherOccupation,FatherEmployer,MotherName,MotherOccupation,MotherEmployer,EarningsFather,EarningsMother,EarningsTotal,FamilyAssets,StudentEmployer,StudentIncome,StudentAssets,Siblings")] Profile profile)


        private bool ProfileExists(int id)
        {
            return _context.Profile.Any(e => e.ProfileId == id);
        }
    }
}
