﻿using Scholarships.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Scholarships.Models
{
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }

        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; } = "";

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; } = "";

        [Required]
        [DisplayName("EAHS Student Id")]
        [StringLength(5, ErrorMessage = "Student ID numbers are five digits long")]
        [RegularExpression(@"[0-9]{5}", ErrorMessage = "Only digits may be used")]
        public string StudentId { get; set; } = "";

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Date of Birth")]
        public DateTime BirthDate { get; set; } = DateTime.Now.AddDays(-365 * 18);

        [Required]
        [Range(0,2, ErrorMessage = "Invalid Gender")] // 0 = male, 1 = female, 2 = other
        public int Gender { get; set; }


        [Required, DataType(DataType.EmailAddress, ErrorMessage="Enter a valid contact email address")]
        public string Email { get; set; }

        [DisplayName("Address (Line 1)")]
        public string Address1 { get; set; } = "";
        [DisplayName("Address (Line 2)")]
        public string Address2 { get; set; } = "";
        public string City { get; set; } = "Easton";
        public string State { get; set; } = "PA";

        [DataType(DataType.PostalCode)]
        [DisplayName("Zip Code")]
        public string ZipCode { get; set; } = "";

        [DataType(DataType.PhoneNumber)]
        [DisplayName("Contact Phone Number")]
        public string Phone { get; set; } = "";

        [DisplayName("Class Rank")]
        public int? ClassRank { get; set; }
        public double? GPA { get; set; }
        [DisplayName("SAT Math Score")]
        public int? SATScoreMath { get; set; }
        [DisplayName("SAT Reading Score")]
        public int? SATScoreReading { get; set; }
        [DisplayName("ACT Score")]
        public int? ACTScore { get; set; }

        [DisplayName("College You Are Attending")]
        public string CollegeAttending { get; set; } = "";
        [DisplayName("Yearly Tuition Cost")]
        public string TuitionYearly { get; set; } = "";
        [DisplayName("Yearly Room and Board Cost")]
        public string RoomBoard { get; set; } = "";
        [DisplayName("Total Yearly Tuition")]
        public string TuitionTotal { get; set; } = "";

        [DisplayName("Have you already been accepted?")]
        public bool CollegeAccepted { get; set; } = false;

        [DisplayName("Intended Major")]
        public string CollegeIntendedMajor { get; set; } = "";

        [RegularExpression(@"oncampus|commute", ErrorMessage = "You must declare your intended living situation")]
        public string LivingSituation { get; set; } = "oncampus"; // "oncampus", "commute"

        [DisplayName("Other Financial Aid")]
        public string OtherAid { get; set; } = "";

        [DisplayName("School Activities")]
        public string ActivitiesSchool { get; set; } = "";
        [DisplayName("Community Activities and Community Service")]
        public string ActivitiesCommunity { get; set; } = "";

        [DisplayName("School Offices and Awards")]
        public string SchoolOffices { get; set; }

        [DisplayName("Special Circumstances")]
        public string SpecialCircumstances { get; set; }

        public List<Guardian> Guardians { get; set; }

        public double EarningsTotal { get; set; }

        public double FamilyAssets { get; set; }
        public string StudentEmployer { get; set; }
        public double StudentIncome { get; set; }

        public double StudentAssets { get; set; }

        public string Siblings { get; set; }

        public bool FormCompletedBasic { get; set; } = false;
        public bool FormCompletedAcademicPerformance { get; set; } = false;
        public bool FormCompletedCollegePlans { get; set; } = false;
        public bool FormCompletedExtraCurriculur { get; set; } = false;
        public bool FormCompletedFamilyIncome { get; set; } = false;
    }
}
