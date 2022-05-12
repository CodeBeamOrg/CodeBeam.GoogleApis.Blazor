namespace GoogleApis.Blazor.Models
{
    public class Creator
    {
        public string id { get; set; }
        public string email { get; set; }
        public string displayName { get; set; }
        public bool self { get; set; }
    }

    public class End
    {
        public string dateTime { get; set; }
        public string timeZone { get; set; }
    }

    public class GoogleCalendarEvent
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

    public class Organizer
    {
        public string id { get; set; }
        public string email { get; set; }
        public string displayName { get; set; }
        public bool self { get; set; }
    }

    public class Reminders
    {
        public bool useDefault { get; set; }
    }

    public class GoogleCalendarEventRoot
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string summary { get; set; }
        //public DateTime updated { get; set; }
        public string timeZone { get; set; }
        public string accessRole { get; set; }
        public List<object> defaultReminders { get; set; }
        public string nextSyncToken { get; set; }
        public List<GoogleCalendarEvent> items { get; set; }
    }

    public class Start
    {
        public string dateTime { get; set; }
        public string timeZone { get; set; }
    }
}
