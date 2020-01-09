using Scholarships.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Tasks
{
    public class GenerateTranscripts : IGenerateTranscripts
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.Configuration Configuration;

        public GenerateTranscripts (ApplicationDbContext context, Services.Configuration configurationService)
        {
            _context = context;
            Configuration = configurationService;
        }

        public void Execute()
        {
            string transcriptPath = Configuration.Get("TranscriptFilePath");
            Log.Information("Beginning processing of graduation transcripts PDF");

            string transcriptSourcePath = Path.Combine(transcriptPath, "transcripts.pdf");

            if (!File.Exists(transcriptSourcePath))
            {
                Log.Error("Executing GenerateTranscripts - Unable to find transcripts.pdf");
            }

            // Calculate the current graduating year for seniors
            int schoolYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            if (currentMonth > 7)
                schoolYear++;

            string transcriptProcessPath = Path.Combine(transcriptPath, schoolYear + "");
            Directory.CreateDirectory(transcriptProcessPath);



            Log.Information("Ending processing of graduation transcripts PDF");
        }
    }
}
