using System.Text.Json;
using System.Text.Json.Serialization;

namespace ADSBRouterSharp
{
    public class TcpConfig
    {
        [JsonPropertyName("input")]
        public HostEntry Input { get; set; }
        [JsonPropertyName("outputs")]
        public List<HostEntry> Outputs { get; set; }

        public static TcpConfig FromFile(string file)
        {
            var config = JsonSerializer.Deserialize<TcpConfig>(File.ReadAllText(file));

            return config is null ? throw new Exception("Failed to read config from file") : config;
        }
    }

    public class HostEntry
    {
        [JsonPropertyName("host")]
        public string Host { get; set; } = "";
        [JsonPropertyName("port")]
        public int Port { get; set; }
    }
}