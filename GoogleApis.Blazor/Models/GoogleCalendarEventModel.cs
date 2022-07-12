using System.Text.Json.Serialization;

namespace GoogleApis.Blazor.Models
#pragma warning disable CS1591
{
    /// <summary>
    /// A root class represents google calendar event api results.
    /// </summary>
    public class GoogleCalendarEventRoot
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "calendar#events";
        
        [JsonPropertyName("etag")]
        public string Etag { get; set; }
        
        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("updated")]
        public string Updated { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("accessRole")]
        public string AccessRole { get; set; }

        [JsonPropertyName("defaultReminders")]
        public List<DefaultReminder> DefaultReminders { get; set; }

        [JsonPropertyName("nextPageToken")]
        public string NextPageToken { get; set; }

        [JsonPropertyName("nextSyncToken")]
        public string NextSyncToken { get; set; }

        [JsonPropertyName("items")]
        public List<GoogleCalendarEventModel> Items { get; set; }
    }

    public class GoogleCalendarEventModel
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "calendar#event";

        [JsonPropertyName("etag")]
        public string Etag { get; set; } = "";

        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("status")]
        public string Status { get; set; } = "confirmed";

        [JsonPropertyName("htmlLink")]
        public string htmlLink { get; set; } = "";

        //public DateTime created { get; set; }
        //public DateTime updated { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        //public string colorId { get; set; }

        [JsonPropertyName("creator")]
        public Creator Creator { get; set; } = new();

        [JsonPropertyName("organizer")]
        public Organizer Organizer { get; set; } = new();

        [JsonPropertyName("start")]
        public Start Start { get; set; } = new();

        [JsonPropertyName("end")]
        public End End { get; set; } = new();

        [JsonPropertyName("endTimeUnspecified")]
        public bool EndTimeUnspecified { get; set; }

        [JsonPropertyName("transparency")]
        public string Transparency { get; set; } = "opaque";

        [JsonPropertyName("visibility")]
        public string Visibility { get; set; } = "default";

        [JsonPropertyName("iCalUID")]
        public string ICalUID { get; set; } = "";

        [JsonPropertyName("sequence")]
        public int Sequence { get; set; }

        [JsonPropertyName("reminders")]
        public Reminders Reminders { get; set; } = new();

        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = "default";
    }
}
