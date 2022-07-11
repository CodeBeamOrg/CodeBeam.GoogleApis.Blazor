namespace GoogleApis.Blazor.Models
#pragma warning disable CS1591
{
    /// <summary>
    /// Represents google calendar resource
    /// </summary>
    public class GoogleCalendarModel
    {
        public string kind { get; private set; } = "calendar#calendar";
        public string etag { get; set; }
        public string id { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public string timeZone { get; set; }
        public ConferenceProperties conferenceProperties { get; set; }
    }    
}
