using Microsoft.EntityFrameworkCore;
using _4VGymAPI.Models;

namespace _4VGymAPI.Data
{
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options)
            : base(options)
        {
        }

        // Indicamos las clases de modelo para mapearlas a tablas de SQL Server
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
    }
}