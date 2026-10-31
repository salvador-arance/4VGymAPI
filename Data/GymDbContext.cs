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

        // Configuración con Fluent API (alternativa a las Data Annotations)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activity>(entity =>
            {
                // Relación 1:N: cada actividad tiene UN tipo y cada tipo tiene MUCHAS actividades
                entity.HasOne(activity => activity.ActivityType).WithMany(activityType => activityType.Activities)
                    .HasForeignKey(activity => activity.ActivityTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Activities_ActivityTypes");
            });
        }
    }
}