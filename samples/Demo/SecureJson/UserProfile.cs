using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecureJson
{
    internal record UserProfile
    {
        [JsonPropertyName("a")]
        [JsonConverter(typeof(GDPRObfuscatedStringConverter))]
        public required string Name { get; set; }

        [JsonPropertyName("b")]
        [JsonConverter(typeof(GDPRObfuscatedStringConverter))]
        public required string Email { get; set; }

        [JsonPropertyName("c")]
        [JsonConverter(typeof(GDPRObfuscatedDateTimeConverter))]
        public DateTime DateOfBirth { get; set; }

        [JsonPropertyName("d")]
        [JsonConverter(typeof(GDPRIPAddressListConverter))]
        public List<IPAddress> Devices { get; set; } = [];
    }
}
