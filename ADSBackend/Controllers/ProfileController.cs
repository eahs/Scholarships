﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

            var fields = await _context.FieldOfStudy.OrderBy(f => f.Name).ToListAsync();
            fields.Insert(0, fields.FirstOrDefault(f => f.FieldOfStudyId == 1));

            ViewBag.Genders = new SelectList(genders, "GenderId", "Name");
            ViewBag.LivingSituations = new SelectList(livingSituations, "Key", "Name");
            ViewBag.States = FormHelper.States;
            ViewBag.FieldsOfStudy = new SelectList(fields, "FieldOfStudyId", "Name");

            return View(profile);
        }

        private async Task<string> UpdateProfile (string bindingfields, Profile profile)
        {
            ScrubModelState(bindingfields);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profile);
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

            return await UpdateProfile(ProfileBindingFields, _profile);
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

            return await UpdateProfile(AcademicBindingFields, _profile);

        }


        private const string CollegePlansBindingFields = "ProfileId,CollegeAttending,TuitionYearly,RoomBoard,TuitionTotal,CollegeAccepted,FieldOfStudy,CollegeIntendedMajor,LivingSituation,OtherAid";

        // POST: Profiles/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> EditCollegePlans(int id, [Bind(CollegePlansBindingFields)] Profile profile)
        {
            var _profile = await GetProfileAsync();

            if (_profile == null || _profile.ProfileId != profile.ProfileId)
            {
                return FormHelper.JsonStatus("NotFound");
            }

            // Copy values from profile into _profile
            _profile.CollegeAttending = profile.CollegeAttending;
            _profile.TuitionYearly = profile.TuitionYearly;
            _profile.RoomBoard = profile.RoomBoard;
            _profile.TuitionTotal = profile.TuitionTotal;
            _profile.CollegeAccepted = profile.CollegeAccepted;
            _profile.FieldOfStudy = profile.FieldOfStudy;
            _profile.CollegeIntendedMajor = profile.CollegeIntendedMajor;
            _profile.LivingSituation = profile.LivingSituation;
            _profile.OtherAid = profile.OtherAid;

            return await UpdateProfile(CollegePlansBindingFields, _profile);

        }

        private const string ExtraCurricularBindingFields = "ProfileId,ActivitiesSchool,ActivitiesCommunity,SchoolOffices,SpecialCircumstances";

        // POST: Profiles/EditExtraCurricular
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> EditExtraCurricular([Bind(ExtraCurricularBindingFields)] Profile profile)
        {
            var _profile = await GetProfileAsync();

            if (_profile == null || _profile.ProfileId != profile.ProfileId)
            {
                return FormHelper.JsonStatus("NotFound");
            }

            // Copy values from profile into _profile
            _profile.ActivitiesSchool = profile.ActivitiesSchool;
            _profile.ActivitiesCommunity = profile.ActivitiesCommunity;
            _profile.SchoolOffices = profile.SchoolOffices;
            _profile.SpecialCircumstances = profile.SpecialCircumstances;

            return await UpdateProfile(ExtraCurricularBindingFields, _profile);

        }


        private void SetupGuardiansAjaxForm ()
        {
            var relations = new[]
            {
                new { Name = "", Id = 0},
                new { Name = "Self", Id = 1},
                new { Name = "Father", Id = 2},
                new { Name = "Mother", Id = 3},
                new { Name = "Guardian", Id = 4}
            }.ToList();
            var employmentStatus = new[]
            {
                new { Name = "", Id = 0},
                new { Name = "Full Time", Id = 1},
                new { Name = "Part Time", Id = 2},
                new { Name = "Unemployed", Id = 3}
            }.ToList();

            ViewBag.Relationships = new SelectList(relations, "Id", "Name");
            ViewBag.EmploymentStatus = new SelectList(employmentStatus, "Id", "Name");
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
                    Relationship = 1 // self
                };
                _context.Guardian.Add(self);
                await _context.SaveChangesAsync();

                guardians = await _context.Guardian.Where(g => g.ProfileId == _profile.ProfileId).ToListAsync();
            }

            SetupGuardiansAjaxForm();

            return View(guardians);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGuardiansAjax ([Bind("ProfileId,GuardianId,FullName,Relationship,EmploymentStatus,Occupation,Employer,AnnualIncome")] ICollection<Guardian> guardians, bool addGuardian = false, int removeGuardianId = -1)
        {
            var _profile = await GetProfileAsync();

            if (removeGuardianId != -1)
            {
                var guardianToRemove = await _context.Guardian.FirstOrDefaultAsync(g => g.ProfileId == _profile.ProfileId && g.GuardianId == removeGuardianId);

                if (guardianToRemove != null)
                {
                    var gr = guardians.FirstOrDefault(g => g.GuardianId == guardianToRemove.GuardianId);
                    if (gr != null)
                        guardians.Remove(gr);

                    TryValidateModel(guardians);

                    _context.Remove(guardianToRemove);
                    await _context.SaveChangesAsync();
                }
            }

            if (addGuardian)
            {
                _context.Guardian.Add(new Guardian
                {
                    ProfileId = _profile.ProfileId,
                    FullName = "",
                    Relationship = 0
                });

                await _context.SaveChangesAsync();
            }

            var _guardians = await _context.Guardian.Where(g => g.ProfileId == _profile.ProfileId).ToListAsync();

            if (ModelState.IsValid)
            {
                foreach (var guardian in _guardians)
                {
                    var fg = guardians.FirstOrDefault(g => g.GuardianId == guardian.GuardianId);

                    if (fg != null && fg.ProfileId == _profile.ProfileId)
                    {
                        guardian.FullName = fg.FullName;
                        guardian.Relationship = fg.Relationship;
                        guardian.EmploymentStatus = fg.EmploymentStatus;
                        guardian.Occupation = fg.Occupation;
                        guardian.Employer = fg.Employer;
                        guardian.AnnualIncome = fg.AnnualIncome;

                        _context.Guardian.Update(guardian);
                    }
                }

                await _context.SaveChangesAsync();
            }

            SetupGuardiansAjaxForm();
            return View(_guardians);
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
