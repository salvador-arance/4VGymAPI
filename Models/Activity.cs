using System.ComponentModel.DataAnnotations.Schema;

namespace _4VGymAPI.Models
{
    public class Activity
    {
        public int Id { get; set; }

        public int MaxParticipants { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        // Relación M:N con los monitores (EF crea la tabla intermedia ActivityInstructor)
        public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();

        // Relación 1:N con el tipo de actividad: clave foránea y propiedad de navegación
        public int? ActivityTypeId { get; set; }

        [ForeignKey(nameof(ActivityTypeId))]
        public ActivityType? ActivityType { get; set; }
    }

}