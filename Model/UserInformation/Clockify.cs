using System.Text.Json.Serialization;

namespace ImportFromTempoToAnotherTool.Model.UserInformation
{
    internal class Clockify
    {
        [JsonPropertyName("api_key")]
        public string? ApiKey { get; set; }

        [JsonPropertyName("projectId")]
        public string? ProjectId { get; set; }
    }
}