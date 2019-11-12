using Scholarships.Models;
using Scholarships.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<ScholarshipCategory> ScholarshipCategory { get; set; }
        public DbSet<Guardian> Guardian { get; set; }

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
        }
    }
}
