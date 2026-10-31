using System.Text.Json.Serialization;

namespace _4VGymAPI.Models
{
    public class Instructor
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("photo_url")]
        public string PhotoUrl { get; set; } = string.Empty;

        // Opcional en la BBDD, aunque el API lo exige al crear un monitor
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        // Propiedad de navegación para la relación M:N entre monitor y actividades
        // Con [JsonIgnore] para evitar ciclos infinitos al serializar JSON
        [JsonIgnore]
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}