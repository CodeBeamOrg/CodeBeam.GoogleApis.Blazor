namespace GoogleApis.Blazor.Models
#pragma warning disable CS1591
{
    /// <summary>
    /// A root class represents google calendar event api results.
    /// </summary>
    public class GoogleCalendarEventRoot
    {
        public string kind { get; set; } = "calendar#events";
        public string etag { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string updated { get; set; }
        public string timeZone { get; set; }
        public string accessRole { get; set; }
        public List<DefaultReminder> defaultReminders { get; set; }
        public string nextPageToken { get; set; }
        public string nextSyncToken { get; set; }
        public List<GoogleCalendarEventModel> items { get; set; }
    }

    public class GoogleCalendarEventModel
    {
        public string kind { get; set; } = "calendar#event";
        public string etag { get; set; } = "";
        public string id { get; set; } = "";
        public string status { get; set; } = "confirmed";
        public string htmlLink { get; set; } = "";
        //public DateTime created { get; set; }
        //public DateTime updated { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        //public string colorId { get; set; }
        public Creator creator { get; set; } = new();
        public Organizer organizer { get; set; } = new();
        public Start start { get; set; } = new();
        public End end { get; set; } = new();
        public bool endTimeUnspecified { get; set; }
        public string transparency { get; set; } = "opaque";
        public string visibility { get; set; } = "default";
        public string iCalUID { get; set; } = "";
        public int sequence { get; set; }
        public Reminders reminders { get; set; } = new();
        public string eventType { get; set; } = "default";
    }
}
