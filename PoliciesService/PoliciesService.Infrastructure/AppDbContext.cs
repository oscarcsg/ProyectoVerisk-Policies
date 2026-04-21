using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PoliciesService.Domain;

namespace PoliciesService.Infrastructure
{
    public class AppDbContext : DbContext
    {
        #region Attributes
        public DbSet<PolicyHolder> PolicyHolders { get; set; }
        public DbSet<Policy> Policies { get; set; }
        #endregion

        #region Constructors
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PolicyHolder config
            modelBuilder.Entity<PolicyHolder>(entity =>
            {
                entity.HasKey(ph => ph.Id);
                entity.Property(ph => ph.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(ph => ph.LastName).HasMaxLength(100).IsRequired();

                entity.Property(ph => ph.Email).HasMaxLength(255).IsRequired();
                entity.HasIndex(ph => ph.Email).IsUnique();

                entity.Property(ph => ph.Phone).HasMaxLength(20);
                entity.Property(ph => ph.RegionCode).HasMaxLength(10).IsRequired();
            });

            // Policy config
            modelBuilder.Entity<Policy>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.PolicyNumber).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.PolicyNumber).IsUnique();

                entity.Property(e => e.PolicyTypeCode).HasMaxLength(20).IsRequired();
                entity.Property(e => e.CoverageTypeCode).HasMaxLength(20).IsRequired();

                entity.Property(e => e.CoverageAmount).HasPrecision(18, 2).IsRequired();
                entity.Property(e => e.PremiumAmount).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();

                entity.HasOne(p => p.PolicyHolder)
                      .WithMany(ph => ph.Policies)
                      .HasForeignKey(p => p.PolicyHolderId)
                      .OnDelete(DeleteBehavior.Cascade); // If the policeholder is deleted, all its policies does too
            });
        }
        #endregion
    }
}
