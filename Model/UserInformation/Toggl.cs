using System.Text.Json.Serialization;

namespace ImportFromTempoToAnotherTool.Model.UserInformation
{
    internal class Toggl
    {
        [JsonPropertyName("usertoken")]
        public string? Token { get; set; }

        [JsonPropertyName("useremail")]
        public string? Email { get; set; }

        [JsonPropertyName("userpassword")]
        public string? Password { get; set; }

        [JsonPropertyName("workspaceid")]
        public int? WorkSpaceId { get; set; }
    }
}