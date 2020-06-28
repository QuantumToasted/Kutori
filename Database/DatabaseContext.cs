using Kutori.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Kutori.Database
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class DatabaseContext : DbContext
    {
        public DbSet<Post> Posts => Set<Post>();

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite("Data Source=./Kutori.db");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Post>(p =>
            {
                p.HasKey(x => x.Id);
                p.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                p.Property(x => x.IP)
                    .HasConversion(x => x.ToString(), x => IPAddress.Parse(x));
            });
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
