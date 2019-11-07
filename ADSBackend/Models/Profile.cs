using System;
using System.ComponentModel.DataAnnotations;

namespace ADSBackend.Models
{
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentId { get; set; }

        public DateTime BirthDate { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        public string Phone { get; set; }

        public int ClassRank { get; set; }
        public int SATScoreMath { get; set; }
        public int SATScoreReading { get; set; }
        public int ACTScore { get; set; }

        public string CollegeAttending { get; set; }
        public double TuitionYearly { get; set; }
        public double RoomBoard { get; set; }
        public double TuitionTotal { get; set; }

        public bool CollegeAccepted { get; set; }
        public string CollegeIntendedMajor { get; set; }

        public string LivingSituation { get; set; }  // "campus", "commute"
        public string OtherAid { get; set; }


        public string ActivitiesSchool { get; set; }
        public string ActivitiesCommunity { get; set; }

        public string SchoolOffices { get; set; }
        public string SpecialCircumstances { get; set; }

        public string FatherName { get; set; }
        public string FatherOccupation { get; set; }
        public string FatherEmployer { get; set; }
        public string MotherName { get; set; }
        public string MotherOccupation { get; set; }
        public string MotherEmployer { get; set; }
        public string EarningsFather { get; set; }
        public double EarningsMother { get; set; }
        public double EarningsTotal { get; set; }


        public double FamilyAssets { get; set; }
        public string StudentEmployer { get; set; }
        public double StudentIncome { get; set; }

        public double StudentAssets { get; set; }

        public string Siblings { get; set; }
    }
}
