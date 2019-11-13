using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scholarships.Data;
using Scholarships.Models;
using Scholarships.Models.Identity;
using Scholarships.Util;

namespace Scholarships.Controllers
{
    [Authorize]
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

        private const string ProfileBindingFields = "ProfileId,FirstName,LastName,StudentId,BirthDate,Gender,Email,Address1,Address2,City,ZipCode,Phone";

        // POST: Profiles/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> EditProfile([Bind(ProfileBindingFields)] Profile profile)
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

            ScrubModelState(ProfileBindingFields);

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

            return FormHelper.JsonStatus(new { Status = "InvalidRequest", Errors = ModelState.Values.Where(i => i.Errors.Count > 0) }); ;
        }

        private const string AcademicBindingFields = "ProfileId,ClassRank,GPA,SATScoreMath,SATScoreReading,ACTScore";

        // POST: Profiles/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> EditAcademic(int id, [Bind(AcademicBindingFields)] Profile profile)
        {
            var _profile = await GetProfileAsync();

            if (_profile == null || _profile.ProfileId != profile.ProfileId)
            {
                return FormHelper.JsonStatus("NotFound");
            }

            // Copy values from profile into _profile
            _profile.ClassRank = profile.ClassRank;
            _profile.GPA = profile.GPA;
            _profile.SATScoreMath = profile.SATScoreMath;
            _profile.SATScoreReading = profile.SATScoreReading;
            _profile.ACTScore = profile.ACTScore;

            ScrubModelState(AcademicBindingFields);

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

            return FormHelper.JsonStatus(new { Status = "InvalidRequest", Errors = ModelState.Values.Where(i => i.Errors.Count > 0) }); ;
        }

        public async Task<IActionResult> EditGuardiansAjax ()
        {
            var _profile = await GetProfileAsync();

            var guardians = await _context.Guardian.Where(g => g.ProfileId == _profile.ProfileId).ToListAsync();

            if (guardians == null || guardians.Count == 0)
            {
                var self = new Guardian
                {
                    ProfileId = _profile.ProfileId,
                    FullName = _profile.FirstName + " " + _profile.LastName,
                    Relationship = 0 // self
                };
                _context.Guardian.Add(self);
                await _context.SaveChangesAsync();

                guardians = await _context.Guardian.Where(g => g.ProfileId == _profile.ProfileId).ToListAsync();
            }

            var relations = new[]
            {
                new { Name = "Self", Id = 0},
                new { Name = "Father", Id = 1},
                new { Name = "Mother", Id = 2},
                new { Name = "Guardian", Id = 2}
            }.ToList();
            var employmentStatus = new[]
            {
                new { Name = "Full-time", Id = 0},
                new { Name = "Part-time", Id = 1},
                new { Name = "Unemployed", Id = 2}
            }.ToList();

            ViewBag.Relationships = new SelectList(relations, "Id", "Name");
            ViewBag.EmploymentStatus = new SelectList(employmentStatus, "Id", "Name");

            return View(guardians);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGuardiansAjax (ICollection<Guardian> guardians)
        {
            await Task.Delay(0);

            return View(guardians);
        }

        //         public async Task<IActionResult> EditAcademic(int id, [Bind("ProfileId,ClassRank,GPA,SATScoreMath,SATScoreReading,ACTScore,CollegeAttending,TuitionYearly,RoomBoard,TuitionTotal,CollegeAccepted,CollegeIntendedMajor,LivingSituation,OtherAid,ActivitiesSchool,ActivitiesCommunity,SchoolOffices,SpecialCircumstances,FatherName,FatherOccupation,FatherEmployer,MotherName,MotherOccupation,MotherEmployer,EarningsFather,EarningsMother,EarningsTotal,FamilyAssets,StudentEmployer,StudentIncome,StudentAssets,Siblings")] Profile profile)

        // Remove any modelstate errors that don't pertain to the actual fields we are binding to
        private void ScrubModelState(string bindingFields)
        {
            string[] bindingKeys = bindingFields.Split(",");
            foreach (string key in ModelState.Keys)
            {
                if (!bindingKeys.Contains(key))
                    ModelState.Remove(key);
            }
        }

        private bool ProfileExists(int id)
        {
            return _context.Profile.Any(e => e.ProfileId == id);
        }
    }
}
