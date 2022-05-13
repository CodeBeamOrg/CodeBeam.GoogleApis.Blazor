namespace GoogleApis.Blazor.Models
#pragma warning disable CS1591
{
    /// <summary>
    /// A root class represents google calendar list api results.
    /// </summary>
    public class GoogleCalendarListRoot
    {
        public string kind { get; set; } = "calendar#calendarList";
        public string etag { get; set; }
        public string nextPageToken { get; set; }
        public string nextSyncToken { get; set; }
        public List<GoogleCalendarListModel> items { get; set; }
    }

    public class GoogleCalendarListModel
    {
        public string kind { get; set; } = "calendar#calendarListEntry";
        public string etag { get; set; }
        public string id { get; set; }
        public string summary { get; set; }
        public string summaryOverride { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public string timeZone { get; set; }
        //public string colorId { get; set; }
        public string backgroundColor { get; set; }
        public string foregroundColor { get; set; }
        public bool hidden { get; set; }
        public bool selected { get; set; }
        public string accessRole { get; set; }
        public List<DefaultReminder> defaultReminders { get; set; }
        public ConferenceProperties conferenceProperties { get; set; }
        public NotificationSettings notificationSettings { get; set; }
        public bool? primary { get; set; }
        public bool deleted { get; set; }
    }
    
}
