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
        public DbSet<ImportedProfile> ImportedProfile { get; set; }
        public DbSet<Scholarship> Scholarship { get; set; }
        public DbSet<ScholarshipFavorite> ScholarshipFavorite { get; set; }
        public DbSet<FieldOfStudy> FieldOfStudy { get; set; }
        public DbSet<ScholarshipCategory> ScholarshipCategory { get; set; }
        public DbSet<ScholarshipFieldOfStudy> ScholarshipFieldOfStudy { get; set; }
        public DbSet<ScholarshipProfileProperty> ScholarshipProfileProperty { get; set; }
        public DbSet<ProfileProperty> ProfileProperty { get; set; }
        public DbSet<Application> Application { get; set; }   // Holds scholarship applications
        public DbSet<AnswerGroup> AnswerGroup { get; set; }  // Holds multiple answer sets for each application
        public DbSet<AnswerGroupSets> AnswerGroupSets { get; set; }
        public DbSet<Guardian> Guardian { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<AnswerSet> AnswerSet  { get; set; }
        public DbSet<Question> Question  { get; set; }
        public DbSet<QuestionOption> QuestionOption  { get; set; }
        public DbSet<QuestionSet> QuestionSet  { get; set; }

        public DbSet<FileAttachmentGroup> FileAttachmentGroup { get; set; }
        public DbSet<FileAttachment> FileAttachment { get; set; }
        public DbSet<Job> Job { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Scholarship category relations

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


            // Scholarship fields of study relations
            builder.Entity<ScholarshipFieldOfStudy>()
                .HasKey(t => new { t.FieldOfStudyId, t.ScholarshipId });

            builder.Entity<ScholarshipFieldOfStudy>()
                .HasOne(nt => nt.Scholarship)
                .WithMany(n => n.FieldsOfStudy)
                .HasForeignKey(nt => nt.ScholarshipId);

            builder.Entity<ScholarshipFieldOfStudy>()
                .HasOne(nt => nt.FieldOfStudy)
                .WithMany(t => t.Scholarships)
                .HasForeignKey(nt => nt.FieldOfStudyId);


            // Scholarship profile relations
            builder.Entity<ScholarshipProfileProperty>()
                .HasKey(t => new { t.ProfilePropertyId, t.ScholarshipId });

            builder.Entity<ScholarshipProfileProperty>()
                .HasOne(nt => nt.Scholarship)
                .WithMany(n => n.ProfileProperties)
                .HasForeignKey(nt => nt.ScholarshipId);

            builder.Entity<ScholarshipProfileProperty>()
                .HasOne(nt => nt.ProfileProperty)
                .WithMany(t => t.Scholarships)
                .HasForeignKey(nt => nt.ProfilePropertyId);

            // Scholarship application answer set relations
            builder.Entity<AnswerGroupSets>()
                .HasKey(t => new { t.AnswerSetId, t.AnswerGroupId });

            builder.Entity<AnswerGroupSets>()
                .HasOne(app => app.AnswerGroup)
                .WithMany(app => app.AnswerSets)
                .HasForeignKey(nt => nt.AnswerGroupId);

            // Scholarship favorites
            builder.Entity<ScholarshipFavorite>()
                .HasKey(t => new {t.ProfileId, t.ScholarshipId});

            builder.Entity<ScholarshipFavorite>()
                .HasOne(fav => fav.Profile)
                .WithMany(fav => fav.Favorites)
                .HasForeignKey(f => f.ProfileId);
        }

        public DbSet<Scholarships.Models.Category> Category { get; set; }

        public DbSet<Scholarships.Models.Article> Article { get; set; }
    }
}
