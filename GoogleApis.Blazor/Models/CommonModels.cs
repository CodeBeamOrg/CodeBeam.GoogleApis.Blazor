using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GoogleApis.Blazor.Models
#pragma warning disable CS1591
{
    public class Creator
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("self")]
        public bool Self { get; set; }
    }

    public class Start
    {
        [JsonPropertyName("dateTime")]
        public string DateTime { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }
    }

    public class End
    {
        [JsonPropertyName("dateTime")]
        public string DateTime { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }
    }

    public class Organizer
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("self")]
        public bool Self { get; set; }
    }

    public class Reminders
    {
        [JsonPropertyName("useDefault")]
        public bool UseDefault { get; set; }
    }

    public class ConferenceProperties
    {
        [JsonPropertyName("allowedConferenceSolutionTypes")]
        public List<string> AllowedConferenceSolutionTypes { get; set; }
    }

    public class DefaultReminder
    {
        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("minutes")]
        public int Minutes { get; set; }
    }

    public class Notification
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }
    }

    public class NotificationSettings
    {
        [JsonPropertyName("notifications")]
        public List<Notification> Notifications { get; set; }
    }


}
