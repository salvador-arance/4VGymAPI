using System.Text.Json.Serialization;
using _4VGymAPI.Models;

namespace _4VGymAPI.Dtos
{
    /// <summary>
    /// Modelo de salida de una actividad, con los objetos completos en vez de los IDs.
    /// </summary>
    public class ActivityResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("activity_type")]
        public ActivityType? ActivityType { get; set; }

        [JsonPropertyName("instructors")]
        public List<Instructor> Instructors { get; set; } = new();

        [JsonPropertyName("max_participants")]
        public int MaxParticipants { get; set; }

        [JsonPropertyName("date_start")]
        public DateTime DateStart { get; set; }

        [JsonPropertyName("date_end")]
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Convierte una entidad Activity (con su tipo y sus monitores ya cargados) en el modelo de salida.
        /// </summary>
        public static ActivityResponse FromEntity(Activity activity)
        {
            return new ActivityResponse
            {
                Id = activity.Id,
                ActivityType = activity.ActivityType,
                Instructors = activity.Instructors.ToList(),
                MaxParticipants = activity.MaxParticipants,
                DateStart = activity.DateStart,
                DateEnd = activity.DateEnd
            };
        }
    }
}