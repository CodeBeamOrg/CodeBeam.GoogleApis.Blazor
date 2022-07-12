using System.Text.Json.Serialization;

namespace GoogleApis.Blazor.Models
#pragma warning disable CS1591
{
    /// <summary>
    /// Represents google calendar resource
    /// </summary>
    public class GoogleCalendarModel
    {
        [JsonPropertyName("kind")]
        public string Kind { get; private set; } = "calendar#calendar";

        [JsonPropertyName("etag")]
        public string Etag { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("conferenceProperties")]
        public ConferenceProperties ConferenceProperties { get; set; }
    }    
}
