using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecureJson
{
    [JsonSerializable(typeof(UserProfile))]
    [JsonSourceGenerationOptions(
              GenerationMode = JsonSourceGenerationMode.Metadata,
              Converters = [typeof(GDPRIPAddressListConverter), typeof(GDPRObfuscatedStringConverter), typeof(GDPRObfuscatedIntConverter), typeof(GDPRObfuscatedDateTimeConverter)],
              DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
              PropertyNameCaseInsensitive = true,
              PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
              WriteIndented = true
  )]
    partial class UserProfileDataConverter : System.Text.Json.Serialization.JsonSerializerContext
    {
    }
}
