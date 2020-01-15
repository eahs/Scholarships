using CsvHelper;
using Scholarships.Data;
using Scholarships.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Tasks.Importer
{
    public class StudentDataImporter : IStudentDataImporter
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.Configuration Configuration;

        public StudentDataImporter(ApplicationDbContext context, Services.Configuration configurationService)
        {
            _context = context;
            Configuration = configurationService;
        }

        private void UpdateStudentProfiles (IEnumerable<Profile> NewProfiles)
        {
            //var existingStudents = await _context.Profile.Where(p => p.)
            foreach (Profile p in NewProfiles)
            {

            }
        }

        public void Execute()
        {
            string studentsPath = Configuration.Get("StudentDataFilePath");
            Log.Information("Beginning processing of student data dump");

            if (studentsPath == null)
            {
                Log.Error("Student data log path configuration variable has not been set by administrator");
                return;
            }

            string studentsSourcePath = Path.Combine(studentsPath, "Student Data.txt");

            if (!File.Exists(studentsSourcePath))
            {
                Log.Error("Executing Student Data Importer - Unable to find 'Student Data.txt'");
                return;
            }

            try
            {
                using (var reader = new StreamReader(studentsSourcePath))
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                    csv.Configuration.RegisterClassMap<FieldMapping>();
                    csv.Configuration.Delimiter = "\t";

                    var records = csv.GetRecords<Profile>();

                    UpdateStudentProfiles(records);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to process student data dump");
            }
        }
    }
}
