using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApis.Blazor.Models
#pragma warning disable CS1591
{
    public class Creator
    {
        public string id { get; set; }
        public string email { get; set; }
        public string displayName { get; set; }
        public bool self { get; set; }
    }

    public class Start
    {
        public string dateTime { get; set; }
        public string timeZone { get; set; }
    }

    public class End
    {
        public string dateTime { get; set; }
        public string timeZone { get; set; }
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

    public class ConferenceProperties
    {
        public List<string> allowedConferenceSolutionTypes { get; set; }
    }

    public class DefaultReminder
    {
        public string method { get; set; }
        public int minutes { get; set; }
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


}
