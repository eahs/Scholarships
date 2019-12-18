using Scholarships.Models.Identity;
using System.Collections.Generic;

namespace Scholarships.Models.HomeViewModels
{
    public class HomeViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Article> Articles { get; set; }
    }
}
