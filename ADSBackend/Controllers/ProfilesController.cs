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
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfilesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Profiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Profile.ToListAsync());
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
                    UserId = user.Id
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

        // POST: Profiles/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProfileId,FirstName,LastName,StudentId,BirthDate,Gender,Email,Address1,Address2,City,ZipCode,Phone,ClassRank,SATScoreMath,SATScoreReading,ACTScore,CollegeAttending,TuitionYearly,RoomBoard,TuitionTotal,CollegeAccepted,CollegeIntendedMajor,LivingSituation,OtherAid,ActivitiesSchool,ActivitiesCommunity,SchoolOffices,SpecialCircumstances,FatherName,FatherOccupation,FatherEmployer,MotherName,MotherOccupation,MotherEmployer,EarningsFather,EarningsMother,EarningsTotal,FamilyAssets,StudentEmployer,StudentIncome,StudentAssets,Siblings")] Profile profile)
        {
            var _profile = await GetProfileAsync();

            if (_profile == null || _profile.ProfileId != profile.ProfileId)
            {
                return NotFound();
            }

            // Copy values from profile into _profile

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

        private bool ProfileExists(int id)
        {
            return _context.Profile.Any(e => e.ProfileId == id);
        }
    }
}
