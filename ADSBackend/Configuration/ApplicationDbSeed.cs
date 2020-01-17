using Scholarships.Data;
using Scholarships.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Scholarships.Configuration
{
    public class ApplicationDbSeed
    {
        private readonly ApplicationDbContext _context;
 
        public ApplicationDbSeed (ApplicationDbContext context)
        {
            _context = context;
           
        }

        public string GetJson (string seedFile)
        {
            var file = System.IO.File.ReadAllText(Path.Combine("Configuration", "SeedData", seedFile));

            return file;
        }

        public void SeedDatabase<TEntity> (string jsonFile, DbSet<TEntity> dbset, bool preserveOrder = false) where TEntity : class
        {
            var records = JsonConvert.DeserializeObject<List<TEntity>>(GetJson(jsonFile));

            if (records?.Count > 0)
            {
                if (!preserveOrder)
                {
                    _context.AddRange(records);
                    _context.SaveChanges();
                }
                else
                {
                    foreach (var record in records)
                    {
                        dbset.Add(record);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public void SeedDatabase ()
        {
            CreateFieldOfStudy();
            CreateProfileProperties();
        }

        private void CreateFieldOfStudy ()
        {            
            var fields = _context.FieldOfStudy.FirstOrDefault();
            if (fields == null)
            {
                SeedDatabase<FieldOfStudy>("fieldofstudies.json", _context.FieldOfStudy);
            }

        }

        private void CreateProfileProperties()
        {
            var properties = _context.ProfileProperty.OrderBy(prop => prop.ProfilePropertyId).ToList();
            if (properties == null || properties.Count == 0)
            {
                SeedDatabase<ProfileProperty>("profileproperties.json", _context.ProfileProperty, true);
            }
            else
            {
                var records = JsonConvert.DeserializeObject<List<ProfileProperty>>(GetJson("profileproperties.json"));
                foreach (var prop in records)
                {
                    var exists = properties.FirstOrDefault(p => p.PropertyKey == prop.PropertyKey);

                    if (exists == null)
                    {
                        _context.ProfileProperty.Add(prop);
                        _context.SaveChanges();
                    }
                }

            }

        }

    }
}
