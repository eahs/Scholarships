﻿using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Scholarships.Data;
using Scholarships.Models;
using Scholarships.Util;
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

        private async Task UpdateStudentProfilesAsync(List<ImportedProfile> NewProfiles)
        {
            // Calculate the current graduating year for seniors
            int schoolYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            if (currentMonth > 7)
                schoolYear++;

            if (NewProfiles.Count > 0)
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM ImportedProfile");

            var existingStudents = await _context.Profile.Include(p => p.User)
                                                                    .Where(p => p.GraduationYear == schoolYear)
                                                                    .ToListAsync();

            foreach (ImportedProfile p in NewProfiles)
            {
                var student = existingStudents.FirstOrDefault(ep => ep.User.UserName == p.Email);

                // Does the student profile already exist?
                if (student != null)
                {
                    bool update = Math.Abs((decimal)(student.GPA - p.GPA)) > 0 ||
                                  student.ClassRank != p.ClassRank ||
                                  student.StudentId != p.StudentId ||
                                  student.LunchStatus != p.LunchStatus;

                    if (student.SATScoreMath != null && student.SATScoreMath != p.SATScoreMath)
                    {
                        update = true;
                    }

                    if (student.SATScoreReading != null && student.SATScoreReading != p.SATScoreReading)
                    {
                        update = true;
                    }

                    if (update)
                    {
                        if (p.SATScoreMath != null) student.SATScoreMath = p.SATScoreMath;
                        if (p.SATScoreReading != null) student.SATScoreReading = p.SATScoreReading;
                        student.ClassRank = p.ClassRank;
                        student.GPA = p.GPA;
                        student.StudentId = p.StudentId;
                        student.LunchStatus = p.LunchStatus;

                        _context.Update(student);
                    }
                }

                // Add this data to the imported profile table
                if (p.SATScoreMath == 0) p.SATScoreMath = null;
                if (p.SATScoreReading == 0) p.SATScoreReading = null;

                _context.ImportedProfile.Add(p);
            }

            await _context.SaveChangesAsync();
        }

        public void Execute()
        {
            string studentsPath = Configuration.ConfigPath.StudentDataPath;

            Log.Information("Beginning processing of student data dump");
            Directory.CreateDirectory(studentsPath);

            if (studentsPath == null)
            {
                Log.Error("Student data log path configuration variable has not been set by administrator");
                return;
            }

            string studentsSourcePath = Path.Combine(studentsPath, "Export.txt");

            if (!File.Exists(studentsSourcePath))
            {
                Log.Error("Executing Student Data Importer - Unable to find 'Export.txt'");
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

                    var records = csv.GetRecords<ImportedProfile>();

                    AsyncHelpers.RunSync(() => UpdateStudentProfilesAsync(records.ToList()));

                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to process student data dump");
            }
        }
    }
}
