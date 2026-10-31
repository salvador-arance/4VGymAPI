using System.Text.Json.Serialization;

namespace _4VGymAPI.Models
{
    public class ActivityType
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("number-monitors")]
        public int NumberMonitors { get; set; } = 1;

        public ActivityType()
        {
        }
        public ActivityType(long id, string name, int numberMonitors)
        {
            Id = id;
            Name = name;
            NumberMonitors = numberMonitors;
        }
    }
}
