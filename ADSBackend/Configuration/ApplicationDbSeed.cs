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

namespace Scholarships.Configuration
{
    public class ApplicationDbSeed
    {
        ApplicationDbContext _context;
 
        public ApplicationDbSeed (ApplicationDbContext context)
        {
            _context = context;
           
        }

        public string GetJson (string seedFile)
        {
            var file = System.IO.File.ReadAllText(Path.Combine("Configuration", "SeedData", seedFile));

            return file;
        }

        public void SeedDatabase<TEntity> (string jsonFile, DbSet<TEntity> dbset) where TEntity : class
        {
            var records = JsonConvert.DeserializeObject<List<TEntity>>(GetJson(jsonFile));

            if (records?.Count > 0)
            {
                records.ForEach(s => dbset.Add(s));
                _context.SaveChanges();
                
            }
        }

        public void SeedDatabase ()
        {
            CreateFieldOfStudy();
        }

        private void CreateFieldOfStudy ()
        {            
            var fields = _context.FieldOfStudy.FirstOrDefault();
            if (fields == null)
            {
                SeedDatabase<FieldOfStudy>("fieldofstudies.json", _context.FieldOfStudy);
            }
        }


    }
}
