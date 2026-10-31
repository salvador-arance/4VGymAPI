using System.Text.Json.Serialization;

namespace _4VGymAPI.Models
{
    public class ErrorResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        public ErrorResponse() { }

        public ErrorResponse(int code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
