namespace Scholarships.Models
{
    public class ScholarshipFavorite
    {
        public int ScholarshipId { get; set; }

        public Scholarship Scholarship { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
    }
}
