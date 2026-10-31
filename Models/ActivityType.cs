using System.Text.Json.Serialization;

namespace _4VGymAPI.Models
{
    public class ActivityType
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int NumberMonitors { get; set; } = 1;

        [JsonIgnore]
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();

    }

}