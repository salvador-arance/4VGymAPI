using System.Text.Json.Serialization;

namespace _4VGymAPI.Models
{
    /// <summary>
    /// Modelo de entrada para crear un monitor. No hay ID porque lo genera el repositorio.
    /// </summary>
    public class NewInstructor
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("photo_url")]
        public string? PhotoUrl { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }
    }
}
