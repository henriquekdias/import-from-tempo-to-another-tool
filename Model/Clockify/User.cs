using System.Text.Json.Serialization;

namespace ImportFromTempoToAnotherTool.Model.Clockify
{
    internal class User
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("defaultWorkspace")]
        public string? DefaultWorkspace { get; set; }

    }
}