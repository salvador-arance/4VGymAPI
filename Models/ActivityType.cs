using System.Text.Json.Serialization;

namespace _4VGymAPI.Models
{
    public class ActivityType
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("number-monitors")]
        public int NumberMonitors { get; set; } = 1;

        // Propiedad de navegación inversa (1 Tipo de actividad -> Muchas Actividades)
        // Con [JsonIgnore] para evitar ciclos infinitos al serializar JSON
        [JsonIgnore]
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();

    }

}