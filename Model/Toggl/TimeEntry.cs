using ImportFromTempoToAnotherTool.Core.Extensions;
using System.Text.Json.Serialization;

namespace ImportFromTempoToAnotherTool.Model.Toggl
{
    internal class TimeEntry
    {
        private int _duration = 0;
        private string? _start = string.Empty;

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("stop")]
        public string? Stop { get; set; }

        [JsonPropertyName("server_deleted_at")]
        public string? ServerDeletedAt { get; set; }

        [JsonPropertyName("wid")]
        public int WorkspaceId { get; set; }

        [JsonPropertyName("created_with")]
        public string CreatedWith { get; private set; }

        [JsonPropertyName("billable")]
        public bool Billable { get; private set; }

        [JsonPropertyName("duration")]
        public int Duration
        {
            get
            {
                return _duration;
            }

            set
            {
                _duration = value;
                this.DurationInHours = value.ConvertSecondsToHours();
            }
        }

        [JsonPropertyName("start")]
        public string? Start
        {
            get
            {
                return _start;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                _start = value;
                this.StartTime = value.ConvertStartTempoStringToDateTime();
            }
        }

        [JsonIgnore]
        public TimeSpan? DurationInHours { get; set; }

        [JsonIgnore]
        public DateTime? StartTime { get; set; }

        public TimeEntry()
        {
            CreatedWith = "Snowball";

            Billable = false;
        }
    }
}