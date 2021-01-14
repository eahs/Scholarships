namespace Scholarships.Models
{
    public class ScholarshipProfileProperty
    {
        public int ScholarshipId { get; set; }
        public Scholarship Scholarship { get; set; }
        public int ProfilePropertyId { get; set; }
        public ProfileProperty ProfileProperty { get; set; }
    }
}
