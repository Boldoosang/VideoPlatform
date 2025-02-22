using Microsoft.EntityFrameworkCore;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Infrastructure {
    public class VideoPlatformContext : DbContext {
        public VideoPlatformContext() { 
        
        }

        public VideoPlatformContext(DbContextOptions<VideoPlatformContext> options) : base(options) {

        }

        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<Episode> Episodes { get; set; }
        public virtual DbSet<Season> Seasons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(sqlServerOptionsAction: options => options.EnableRetryOnFailure());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //modelBuilder.Entity<test>()
            //    .HasKey(t => t.ID);

            //modelBuilder.Entity<test>()
            //    .HasIndex(t => t.ID)
            //    .IsUnique();
        }
    }
}
