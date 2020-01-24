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
            Map(m => m.FirstName).Name("FirstName");
            Map(m => m.LastName).Name("LastName");
            Map(m => m.MiddleName).Name("MiddleName");
            Map(m => m.StudentId).Name("StudentNumber");
            Map(m => m.Email).Name("NETWORKID").ConvertUsing(row => row.GetField("NETWORKID") + "@roverkids.org");
            Map(m => m.LunchStatus).Name("LunchStatus");
            Map(m => m.BirthDate).Name("BirthDate");
            Map(m => m.GraduationYear).Name("GraduationYear");
            Map(m => m.Gender).Name("Gender").ConvertUsing(row => row.GetField("Gender") == "M" ? 0 : 1);
            Map(m => m.Ethnicity).Name("Ethnicity").ConvertUsing(row => ConvertFieldToInt(row.GetField("Ethnicity")));
            Map(m => m.Address1).Name("AddressLine1");
            Map(m => m.City).Name("City");
            Map(m => m.State).Name("State");
            Map(m => m.ZipCode).Name("Zip");
            Map(m => m.Phone).Name("PhoneNumber");
            
            
            Map(m => m.ClassRank).Name("Rank").ConvertUsing(row =>
            {
                string rank = row.GetField("Rank");
                string[] parts = rank.Split(' ');

                int classrank;
                bool valid = Int32.TryParse(parts[0], out classrank);

                if (!valid) return 0;

                return classrank;
            });
            
            Map(m => m.GPA).Name("GPA").ConvertUsing(row => ConvertFieldToDouble(row.GetField("GPA")) );
            Map(m => m.SATScoreMath).Name("SATScoreMath").ConvertUsing(row => SplitGetLargest(row.GetField("SATScoreMath")));
            Map(m => m.SATScoreReading).Name("SATScoreEBRW").ConvertUsing(row => SplitGetLargest(row.GetField("SATScoreEBRW")));
            
        }

        private static int SplitGetLargest(string num)
        {
            if (string.IsNullOrEmpty(num))
                return 0;

            string[] parts = num.Replace(" ","").Split(';');
            List<int> scores = new List<int>();

            scores.Add(0);

            foreach (string part in parts)
            {
                int cnum;
                bool converted = Int32.TryParse(part, out cnum);

                if (converted)
                    scores.Add(cnum);
            }

            return scores.Max();
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
