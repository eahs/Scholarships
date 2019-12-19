using Scholarships.Models.Identity;
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

        [DisplayName("Middle Name")]
        public string MiddleName { get; set; } = "";

        [Required]
        [DisplayName("EAHS Student Id")]
        [StringLength(5, ErrorMessage = "Student ID numbers are five digits long")]
        [RegularExpression(@"[0-9]{5}", ErrorMessage = "Student ID's are five digits long. Example: 12345")]
        public string StudentId { get; set; } = "";

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Date of Birth")]
        public DateTime BirthDate { get; set; } = DateTime.Now.AddDays(-365 * 18);

        [Required]
        [Range(0,2, ErrorMessage = "Invalid Gender")] // 0 = male, 1 = female, 2 = other
        public int Gender { get; set; }


        [Required, DataType(DataType.EmailAddress, ErrorMessage="Enter a valid contact email address")]
        [RegularExpression("^(.+)@(?!roverkids.org).+$", ErrorMessage = "Please enter a personal email address you will have access to after graduation - roverkids.org accounts are not allowed")]
        [DisplayName("Personal Email")]
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

        [DisplayName("College You Are Planning to Attend")]
        public string CollegeAttending { get; set; } = "";
        [DisplayName("Yearly Tuition Cost")]
        [DataType(DataType.Currency, ErrorMessage="You must enter the yearly tuition cost in dollars. Example: 35,000")]
        public double TuitionYearly { get; set; }
        [DisplayName("Yearly Room and Board Cost")]
        [DataType(DataType.Currency)]
        public double RoomBoard { get; set; }
        [DisplayName("Total Yearly Cost")]
        [DataType(DataType.Currency)]
        public double TuitionTotal { get; set; }

        [DisplayName("Have you already been accepted?")]
        public bool CollegeAccepted { get; set; } = false;

        [DisplayName("Other Colleges Under Consideration")]
        public string OtherColleges { get; set; } = "";

        [DisplayName("Intended Major")]
        public string CollegeIntendedMajor { get; set; } = "";

        [DisplayName("General Field of Study")]
        public int? FieldOfStudyId { get; set; } = 1;
        public FieldOfStudy FieldOfStudy { get; set; }

        [DisplayName("College Housing Plans")]
        [RegularExpression(@"oncampus|commute", ErrorMessage = "You must declare your intended plans for college housing.")]
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

        [DataType(DataType.Currency)]
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
