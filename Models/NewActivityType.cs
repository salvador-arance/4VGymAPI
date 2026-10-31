using System.Text.Json.Serialization;

namespace _4VGymAPI.Models
{
    public class NewActivityType
    {
        // No hay ID porque nunca me vendrá

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        // Es opcional, porque no tiene porque venir en la peticion POST
        [JsonPropertyName("number-monitors")]
        public int? NumberMonitors { get; set; } = 1;

        public NewActivityType() { }

        public NewActivityType(string name, int numberMonitors)
        {
            Name = name;
            NumberMonitors = numberMonitors;
        }
    }
}
