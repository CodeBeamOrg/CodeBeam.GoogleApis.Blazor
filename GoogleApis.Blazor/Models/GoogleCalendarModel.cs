namespace GoogleApis.Blazor.Models
{
    public class ConferenceProperties
    {
        public List<string> allowedConferenceSolutionTypes { get; set; }
    }

    public class DefaultReminder
    {
        public string method { get; set; }
        public int minutes { get; set; }
    }

    public class GoogleCalendarModel
    {
        // should be location parameter?
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string timeZone { get; set; }
        public string colorId { get; set; }
        public string backgroundColor { get; set; }
        public string foregroundColor { get; set; }
        public bool selected { get; set; }
        public string accessRole { get; set; }
        public List<DefaultReminder> defaultReminders { get; set; }
        public ConferenceProperties conferenceProperties { get; set; }
        public NotificationSettings notificationSettings { get; set; }
        public bool? primary { get; set; }
    }

    public class Notification
    {
        public string type { get; set; }
        public string method { get; set; }
    }

    public class NotificationSettings
    {
        public List<Notification> notifications { get; set; }
    }

    public class GoogleCalendarRoot
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextSyncToken { get; set; }
        public List<GoogleCalendarModel> items { get; set; }
    }
}
