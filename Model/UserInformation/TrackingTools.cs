using System.Text.Json.Serialization;

namespace ImportFromTempoToAnotherTool.Model.UserInformation
{
    internal class TrackingTools
    {
        [JsonPropertyName("toggl")]
        public Toggl? Toggl { get; set; }

        [JsonPropertyName("clockify")]
        public Clockify? Clockify { get; set; }

        public TrackingTools()
        {
            Toggl = new();
            Clockify = new();
        }
    }
}