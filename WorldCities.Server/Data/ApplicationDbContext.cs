using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Data.Models;

namespace WorldCities.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options)
        {
            
        }

        public DbSet<City>  Cities => Set<City>();
        public DbSet<Country> Countries => Set<Country>();

        /* // EntityTypeConfiguration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // add the EntityTypeConfiguration classes
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly );
        }
        */

        /* // Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>().ToTable("Cities");
            modelBuilder.Entity<City>().HasKey(x => x.Id);
            modelBuilder.Entity<City>().Property(x => x.Id).IsRequired();
            modelBuilder.Entity<City>().Property(x => x.Lat).HasColumnType("decimal(7,4)");
            modelBuilder.Entity<City>().Property(x => x.Lon).HasColumnType("decimal(7,4)");

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Country>().HasKey(x => x.Id);
            modelBuilder.Entity<Country>().Property(x => x.Id).IsRequired();
            modelBuilder.Entity<City>()
                .HasOne(x => x.Country)
                .WithMany(y => y.Cities)
                .HasForeignKey(x => x.CountryId);
        }
        */
    }
}
