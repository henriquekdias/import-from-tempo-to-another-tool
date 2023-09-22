using System.Text.Json.Serialization;
using ImportFromTempoToAnotherTool.Core.Extensions;

namespace ImportFromTempoToAnotherTool.Model.Clockify
{
    internal class TimeEntry
    {
        private string? _start = string.Empty;
        private string? _end = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("end")]
        public string? End
        {
            get
            {
                return _end;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                _end = value;
                this.EndTime = value.ConvertStartTempoStringToDateTime();
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

        [JsonPropertyName("projectId")]
        public string? ProjectId { get; set; }

        [JsonPropertyName("timeInterval")]
        public TimeInterval TimeInterval { get; set; }

        [JsonIgnore]
        public TimeSpan? DurationInHours { get; set; }

        [JsonIgnore]
        public DateTime? StartTime { get; set; }

        [JsonIgnore]
        public DateTime? EndTime { get; set; }

        public TimeEntry()
        {
            TimeInterval = new();
        }
    }
}