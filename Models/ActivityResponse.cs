using System.Text.Json.Serialization;

namespace _4VGymAPI.Models
{
    /// <summary>
    /// Modelo de salida de una actividad, con los objetos completos en vez de los IDs.
    /// </summary>
    public class ActivityResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

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
    }
}
