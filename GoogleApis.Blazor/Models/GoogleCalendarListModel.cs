using System.Text.Json.Serialization;

namespace GoogleApis.Blazor.Models
#pragma warning disable CS1591
{
    /// <summary>
    /// A root class represents google calendar list api results.
    /// </summary>
    public class GoogleCalendarListRoot
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "calendar#calendarList";

        [JsonPropertyName("etag")]
        public string Etag { get; set; }

        [JsonPropertyName("nextPageToken")]
        public string NextPageToken { get; set; }

        [JsonPropertyName("nextSyncToken")]
        public string NextSyncToken { get; set; }

        [JsonPropertyName("items")]
        public List<GoogleCalendarListModel> Items { get; set; }
    }

    public class GoogleCalendarListModel
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "calendar#calendarListEntry";

        [JsonPropertyName("etag")]
        public string Etag { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("summaryOverride")]
        public string SummaryOverride { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        //public string colorId { get; set; }

        [JsonPropertyName("backgroundColor")]
        public string BackgroundColor { get; set; }

        [JsonPropertyName("foregroundColor")]
        public string ForegroundColor { get; set; }

        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }

        [JsonPropertyName("selected")]
        public bool Selected { get; set; }

        [JsonPropertyName("accessRole")]
        public string AccessRole { get; set; }

        [JsonPropertyName("defaultReminders")]
        public List<DefaultReminder> DefaultReminders { get; set; }

        [JsonPropertyName("conferenceProperties")]
        public ConferenceProperties ConferenceProperties { get; set; }

        [JsonPropertyName("notificationSettings")]
        public NotificationSettings NotificationSettings { get; set; }

        [JsonPropertyName("primary")]
        public bool? Primary { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }
    }
    
}
