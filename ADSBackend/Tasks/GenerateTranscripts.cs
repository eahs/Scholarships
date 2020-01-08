using Scholarships.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Tasks
{
    public class GenerateTranscripts : IGenerateTranscripts
    {
        private readonly ApplicationDbContext _context;

        public GenerateTranscripts (ApplicationDbContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            for (int i = 0; i < 30; i++)
            {

            }
        }
    }
}
