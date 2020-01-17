using CsvHelper.Configuration;
using Scholarships.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Tasks.Importer
{
    public class FieldMapping : ClassMap<ImportedProfile>
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
            
            Map(m => m.ClassRank).Name("classrank").ConvertUsing(row =>
            {
                string rank = row.GetField("classrank");
                string[] parts = rank.Split(' ');

                int classrank;
                bool valid = Int32.TryParse(parts[0], out classrank);

                if (!valid) return 0;

                return classrank;
            });
            
            Map(m => m.GPA).Name("gpa").ConvertUsing(row => ConvertFieldToDouble(row.GetField("gpa")) );
            Map(m => m.SATScoreMath).Name("sat_math").ConvertUsing(row => ConvertFieldToInt(row.GetField("sat_math")));
            Map(m => m.SATScoreReading).Name("sat_ebrw").ConvertUsing(row => ConvertFieldToInt(row.GetField("sat_ebrw")));

        }

        private static int ConvertFieldToInt(string num)
        {
            if (string.IsNullOrEmpty(num))
                return 0;

            int cnum;
            bool converted = Int32.TryParse(num, out cnum);

            if (converted)
                return cnum;

            return 0;
        }

        private static double ConvertFieldToDouble(string num)
        {
            if (string.IsNullOrEmpty(num))
                return 0;

            double cnum;
            bool converted = Double.TryParse(num, out cnum);

            if (converted)
                return cnum;

            return 0;
        }

    }


}
