using System.Text.Json.Serialization;

namespace _4VGymAPI.Models
{
    public class Instructor
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("photo_url")]
        public string PhotoUrl { get; set; } = string.Empty;

        // Opcional en la BBDD, aunque el API lo exige al crear un monitor
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        public Instructor() { }

        public Instructor(long id, string name, string email, string photoUrl, string? phone)
        {
            Id = id;
            Name = name;
            Email = email;
            PhotoUrl = photoUrl;
            Phone = phone;
        }
    }
}
