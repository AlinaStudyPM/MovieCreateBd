using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Movie> Movies;
        public DbSet<Tag> Tags;
        public DbSet<Actor> Actors;

        public ApplicationContext(bool ToDelete)
        {
            Movies = Set<Movie>();
            Tags = Set<Tag>();
            Actors = Set<Actor>();
            if (ToDelete)
            {
                //Database.EnsureDeleted();
            }
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actor>().HasKey(a => a.Code);
            modelBuilder.Entity<Tag>().HasKey(t => t.Code);
            modelBuilder.Entity<Movie>().HasKey(m => m.Code);

            modelBuilder.Entity<Movie>().HasMany(ma => ma.Actors).WithMany().UsingEntity(j => j.ToTable("MovieActor"));
            modelBuilder.Entity<Movie>().HasMany(md => md.Director).WithMany().UsingEntity(j => j.ToTable("Directors"));
            modelBuilder.Entity<Movie>().HasMany(mt => mt.Tags).WithMany().UsingEntity(j => j.ToTable("MovieTags"));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=file1.db");
        }
    }
}
