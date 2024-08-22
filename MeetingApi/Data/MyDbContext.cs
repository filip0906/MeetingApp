using Microsoft.EntityFrameworkCore;
using MeetingApi.Models;

namespace MeetingApi.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
        {
        }

        public DbSet<Meeting> Meetings { get; set; }
    }
}
