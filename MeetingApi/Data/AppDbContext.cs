using Microsoft.EntityFrameworkCore;
using MeetingApi.Models;

namespace MeetingApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Invitee> Invitees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Meeting>()
                .HasMany(m => m.Invitees)
                .WithOne()
                .HasForeignKey("MeetingId");
        }
    }
}
