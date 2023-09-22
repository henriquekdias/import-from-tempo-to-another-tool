using ImportFromTempoToAnotherTool.Core.Extensions;
using System.Text.Json.Serialization;

namespace ImportFromTempoToAnotherTool.Model.Clockify
{
    internal class TimeInterval
    {
        private string? _start = string.Empty;
        private string? _end = string.Empty;

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

        [JsonIgnore]
        public DateTime? StartTime { get; set; }

        [JsonIgnore]
        public DateTime? EndTime { get; set; }
    }
}