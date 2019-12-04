using Scholarships.Models;
using Scholarships.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Scholarships.Models.Forms;

namespace Scholarships.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ConfigurationItem> ConfigurationItem { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<Scholarship> Scholarship { get; set; }
        public DbSet<FieldOfStudy> FieldOfStudy { get; set; }
        public DbSet<ScholarshipCategory> ScholarshipCategory { get; set; }
        public DbSet<ScholarshipFieldOfStudy> ScholarshipFieldOfStudy { get; set; }
        public DbSet<Guardian> Guardian { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<AnswerOption> AnswerOption  { get; set; }
        public DbSet<AnswerSet> AnswerSet  { get; set; }
        public DbSet<Question> Question  { get; set; }
        public DbSet<QuestionOption> QuestionOption  { get; set; }
        public DbSet<QuestionSet> QuestionSet  { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // intermediate NotificationTags table
            builder.Entity<ScholarshipCategory>()
                .HasKey(t => new { t.CategoryId, t.ScholarshipId });

            builder.Entity<ScholarshipCategory>()
                .HasOne(nt => nt.Scholarship)
                .WithMany(n => n.Categories)
                .HasForeignKey(nt => nt.ScholarshipId);

            builder.Entity<ScholarshipCategory>()
                .HasOne(nt => nt.Category)
                .WithMany(t => t.Scholarships)
                .HasForeignKey(nt => nt.CategoryId);


            builder.Entity<ScholarshipFieldOfStudy>()
                .HasKey(t => new { t.FieldOfStudyId, t.ScholarshipId });

            builder.Entity<ScholarshipFieldOfStudy>()
                .HasOne(nt => nt.Scholarship)
                .WithMany(n => n.FieldsOfStudies)
                .HasForeignKey(nt => nt.ScholarshipId);

            builder.Entity<ScholarshipFieldOfStudy>()
                .HasOne(nt => nt.FieldOfStudy)
                .WithMany(t => t.Scholarships)
                .HasForeignKey(nt => nt.FieldOfStudyId);
        }

        public DbSet<Scholarships.Models.Category> Category { get; set; }
    }
}
