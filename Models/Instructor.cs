using System.Text.Json.Serialization;

namespace _4VGymAPI.Models
{
    public class Instructor
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhotoUrl { get; set; } = string.Empty;

        public string? Phone { get; set; }

        [JsonIgnore]
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();

    }
}