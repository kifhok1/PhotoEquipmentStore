using System.Text.Json.Serialization;

namespace PhotoEquipmentStore.Infrastructure.Connection;

public class ConnectionSettings
{
    [JsonPropertyName("host")]
    public string Host { get; set; }

    [JsonPropertyName("user")]
    public string User { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("database")]
    public string Database { get; set; }

    public override string ToString() =>
        $"host={Host};uid={User};pwd={Password};database={Database}";
}