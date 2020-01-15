using CsvHelper.Configuration;
using Scholarships.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Tasks.Importer
{
    public class FieldMapping : ClassMap<Profile>
    {
        public FieldMapping()
        {
            Map(m => m.FirstName).Name("first_name");
            Map(m => m.LastName).Name("last_name");
            Map(m => m.MiddleName).Name("middle_name");
            Map(m => m.StudentId).Name("student_number");
            Map(m => m.Email).Name("networkid").ConvertUsing(row => row.GetField("networkid") + "@roverkids.org");
            Map(m => m.BirthDate).Name("dob");
            Map(m => m.GraduationYear).Name("graduation_year");
            Map(m => m.Gender).Name("gender").ConvertUsing(row => row.GetField("gender") == "M" ? 0 : 1);
            Map(m => m.Ethnicity).Name("ethnicity");
            Map(m => m.Address1).Name("mailing_street");
            Map(m => m.City).Name("mailing_city");
            Map(m => m.State).Name("mailing_state");
            Map(m => m.ZipCode).Name("mailing_zip");
            Map(m => m.Phone).Name("home_phone");
            Map(m => m.ClassRank).Name("classrank");
            Map(m => m.GPA).Name("gpa");
            Map(m => m.SATScoreMath).Name("sat_math");
            Map(m => m.SATScoreReading).Name("sat_ebrw");

        }
    }
}
