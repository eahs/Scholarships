using Scholarships.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronPdf;
using System.Text.RegularExpressions;

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

            PdfDocument PDF = PdfDocument.FromFile(transcriptSourcePath);
            
            Dictionary<string, List<int>> studentIndex = new Dictionary<string, List<int>>();
            for (int i = 0; i < PDF.PageCount; i++)
            {
                string text = "";

                try
                {
                    text = PDF.ExtractTextFromPage(i);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error extracting pdf text");
                }

                Match m = Regex.Match(text, "Student Number: ([0-9]{5})", RegexOptions.Multiline);

                if (m.Groups.Count > 1)
                {
                    string id = m.Groups[1].Value;

                    if (id.Length != 5) continue;

                    if (!studentIndex.ContainsKey(id))
                        studentIndex.Add(id, new List<int>());

                    studentIndex[id].Add(i);
                }
            }

            foreach (string id in studentIndex.Keys)
            {
                PdfDocument transcript = PDF.CopyPages(studentIndex[id]);

                string transcriptSavePath = Path.Combine(transcriptProcessPath, id + ".pdf");

                try
                {
                    transcript.SaveAs(transcriptSavePath);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unable to save pdf file: {0}", transcriptSavePath);
                }
            }

            Log.Information("Ending processing of graduation transcripts PDF");
        }
    }
}
