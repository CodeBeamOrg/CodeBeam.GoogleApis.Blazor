using GoogleApis.Blazor.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoogleApis.Blazor.Calendar
{
    /// <summary>
    /// Makes Google Calendar Api calls.
    /// </summary>
    public class CalendarService
    {
        [Inject] IHttpClientFactory HttpClientFactory { get; set; }

        /// <summary>
        /// Constructs the class.
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public CalendarService(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        private string _accessToken = "";

        /// <summary>
        /// The access token using throughout the class.
        /// </summary>
        public string AccessToken 
        {
            get => _accessToken;
            set => _accessToken = value; 
        }

        #region Calendars

        /// <summary>
        /// Get all calendars that authenticated user has.
        /// </summary>
        /// <returns></returns>
        public string GetCalendars()
        {
            var client = HttpClientFactory.CreateClient();
            //This uses calendar list instead of calendar. Read for the difference https://developers.google.com/calendar/api/concepts/events-calendars#calendar_and_calendar_list
            var result = client.GetAsync("https://www.googleapis.com/calendar/v3/users/me/calendarList?access_token=" + _accessToken).Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Get calendar with given id that authenticated user has.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string GetCalendarById(string calendarId)
        {
            var client = HttpClientFactory.CreateClient();

            var result = client.GetAsync($"https://www.googleapis.com/calendar/v3/users/me/calendarList/{calendarId}?access_token=" + _accessToken).Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Get calendar metadata with given summary.
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        public string GetCalendarBySummary(string summary)
        {
            string calendars = GetCalendars();
            if (calendars == "error")
            {
                return "error: Can't fetch calendars.";
            }
            GoogleCalendarRoot jsonCalendar = JsonSerializer.Deserialize<GoogleCalendarRoot>(calendars);

            if (jsonCalendar.items == null)
            {
                return "none";
            }

            string calendarId = "";
            foreach (var item in jsonCalendar.items)
            {
                if (item.summary == summary)
                {
                    calendarId = item.id;
                    break;
                }
            }

            if (string.IsNullOrEmpty(calendarId))
            {
                return "none";
            }

            return GetCalendarById(calendarId);
        }

        /// <summary>
        /// Add a new secondary calendar into user's CalendarList.
        /// </summary>
        /// <param name="accessToken">abc</param>
        /// <returns>Returns the request's result content.</returns>
        public string AddCalendar(string accessToken, string calendarTitle, string description = "", string timeZone = "")
        {
            GoogleCalendarModel calendar = new()
            {
                summary = calendarTitle,
                description = description,
                timeZone = timeZone,
            };

            string requestBody = JsonSerializer.Serialize(calendar);

            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var result = client.PostAsync($"https://www.googleapis.com/calendar/v3/calendars", content).Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        public string UpdateCalendar(string accessToken, string calendarId, string calendarTitle, string description = "", string timeZone = "")
        {
            GoogleCalendarModel calendar = new()
            {
                summary = calendarTitle,
                description = description,
                timeZone = timeZone,
            };

            string requestBody = JsonSerializer.Serialize(calendar);

            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var result = client.PutAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}", content).Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Deletes the specified calendar. Only deletes the secondary calendars. Use Clear method for primary calendar.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string DeleteCalendar(string calendarId)
        {
            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var result = client.DeleteAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}").Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Deletes all events in a primary calendar. If it's a secondary calendar, it has no effect. Use DeleteCalendar for secondary ones.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string ClearCalendar(string calendarId)
        {
            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var result = client.PostAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/clear", content).Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        #endregion


        #region Events

        /// <summary>
        /// Get event list within a specific calendar between the specified dates. Returns a max of 2500 events.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="timeMin"></param>
        /// <param name="timeMax"></param>
        /// <returns></returns>
        public string GetEvents(DateTime timeMin, DateTime timeMax, string calendarId)
        {
            var client = HttpClientFactory.CreateClient();
            var result = client.GetAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events?access_token={_accessToken}&maxResults=2500").Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Get event in a specific calendar with a given id.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public string GetEventById(string eventId, string calendarId)
        {
            var client = HttpClientFactory.CreateClient();
            var result = client.GetAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events/{eventId}?access_token=" + _accessToken).Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Creates a new event in a specific calendar.
        /// </summary>
        /// <param name="calendarEvent"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string AddEvent(GoogleCalendarEvent calendarEvent, string calendarId)
        {
            string requestBody = JsonSerializer.Serialize(calendarEvent);

            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = client.PostAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events", content).Result;
            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Updates the event. If overwrite is false (by default), it only replaces the specified values, other values will remain same.
        /// </summary>
        /// <param name="newCalendarEvent"></param>
        /// <param name="eventId"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string UpdateEvent(GoogleCalendarEvent newCalendarEvent, string eventId, string calendarId)
        {
            string requestBody = JsonSerializer.Serialize(newCalendarEvent);

            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            HttpResponseMessage result = new();
            // TO DO: Add patch semantics with overwrite parameter.
            //if (overwrite == false)
            //{
            //    result = client.PatchAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events/{eventId}", content).Result;

            //}
            //else (overwrite == true)
            //{
            //    result = client.PutAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events/{eventId}", content).Result;
            //}
            result = client.PutAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events/{eventId}", content).Result;
            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Finds event in specified calendar with given min and max time period and the summary.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string GetEventBySummary(string summary, DateTime dateMin, DateTime dateMax, string calendarId)
        {
            string events = GetEvents(dateMin, dateMax, calendarId);
            if (events == "error")
            {
                return "error: Can't fetch calendars.";
            }
            GoogleCalendarEventRoot jsonEvent = JsonSerializer.Deserialize<GoogleCalendarEventRoot>(events);

            if (jsonEvent.items == null)
            {
                return "none";
            }

            string eventId = "";
            foreach (var item in jsonEvent.items)
            {
                if (item.summary == summary)
                {
                    eventId = item.id;
                }
            }

            if (string.IsNullOrEmpty(eventId))
            {
                return "none";
            }

            return GetEventById(eventId, calendarId);
        }

        /// <summary>
        /// Finds the event with given summary in given GoogleCalendarEventRoot.items collection.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="googleCalendarEventRoot"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string GetEventBySummary(string summary, GoogleCalendarEventRoot googleCalendarEventRoot, string calendarId)
        {
            string eventId = "";
            if (googleCalendarEventRoot.items == null)
            {
                return "null";
            }
            foreach (var calendarEvent in googleCalendarEventRoot.items)
            {
                if (calendarEvent.summary == summary)
                {
                    eventId = calendarEvent.id;
                }
            }

            if (string.IsNullOrEmpty(eventId))
            {
                return "none";
            }

            return GetEventById(eventId, calendarId);
        }

        /// <summary>
        /// Finds event in specified calendar with given min and max time period and the description.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string GetEventByDescription(string description, DateTime dateMin, DateTime dateMax, string calendarId)
        {
            string events = GetEvents(dateMin, dateMax, calendarId);
            if (events == "error")
            {
                return "error: Can't fetch calendars.";
            }
            GoogleCalendarEventRoot jsonEvent = JsonSerializer.Deserialize<GoogleCalendarEventRoot>(events);

            if (jsonEvent.items == null)
            {
                return "none";
            }

            string eventId = "";
            foreach (var item in jsonEvent.items)
            {
                if (item.description == description)
                {
                    eventId = item.id;
                }
            }

            if (string.IsNullOrEmpty(eventId))
            {
                return "none";
            }

            return GetEventById(eventId, calendarId);
        }

        /// <summary>
        /// Finds the event with given description in given GoogleCalendarEventRoot.items collection.
        /// <param name="description"></param>
        /// <param name="googleCalendarEventRoot"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string GetEventByDescription(string description, GoogleCalendarEventRoot googleCalendarEventRoot, string calendarId)
        {
            string eventId = "";
            if (googleCalendarEventRoot.items == null)
            {
                return "null";
            }
            foreach (var calendarEvent in googleCalendarEventRoot.items)
            {
                if (calendarEvent.description == description)
                {
                    eventId = calendarEvent.id;
                }
            }

            if (string.IsNullOrEmpty(eventId))
            {
                return "none";
            }

            return GetEventById(eventId, calendarId);
        }

        /// <summary>
        /// Deletes the event in a specific calendar.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string DeleteEvent(string eventId, string calendarId)
        {
            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var result = client.DeleteAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events/{eventId}").Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        #endregion


        #region Utilities

        /// <summary>
        /// Finds and returns the calendar id which matches with the given value.
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public string FindCalendarId(CalendarValueType valueType, object val)
        {
            //string result = GetCalendarBySummary(summary);
            //GoogleCalendarModel jsonCalendar = JsonSerializer.Deserialize<GoogleCalendarModel>(result);

            //if (string.IsNullOrEmpty(jsonCalendar.id))
            //{
            //    return "none";
            //}
            //return jsonCalendar.id;

            string calendars = GetCalendars();
            if (calendars == "error")
            {
                return "error: Can't fetch calendars.";
            }
            GoogleCalendarRoot googleCalendarRoot = JsonSerializer.Deserialize<GoogleCalendarRoot>(calendars);

            if (googleCalendarRoot.items == null)
            {
                return "none";
            }

            string calendarId = "";
            if (valueType == CalendarValueType.Summary)
            {
                foreach (var item in googleCalendarRoot.items)
                {
                    if (item.summary == val.ToString())
                    {
                        calendarId = item.id;
                        break;
                    }
                }
            }
            else if (valueType == CalendarValueType.Description)
            {
                foreach (var item in googleCalendarRoot.items)
                {
                    if (item.description == val.ToString())
                    {
                        calendarId = item.id;
                        break;
                    }
                }
            }
            else if (valueType == CalendarValueType.Location)
            {
                foreach (var item in googleCalendarRoot.items)
                {
                    if (item.location == val.ToString())
                    {
                        calendarId = item.id;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(calendarId))
            {
                return "none";
            }
            else
            {
                return calendarId;
            }

        }

        /// <summary>
        /// Finds and returns the event id which matches with the given value and type.
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public string FindEventId(EventValueType valueType, object val)
        {
            //string result = GetCalendarBySummary(summary);
            //GoogleCalendarModel jsonCalendar = JsonSerializer.Deserialize<GoogleCalendarModel>(result);

            //if (string.IsNullOrEmpty(jsonCalendar.id))
            //{
            //    return "none";
            //}
            //return jsonCalendar.id;

            string calendars = GetCalendars();
            if (calendars == "error")
            {
                return "error: Can't fetch calendars.";
            }
            GoogleCalendarRoot googleCalendarRoot = JsonSerializer.Deserialize<GoogleCalendarRoot>(calendars);

            if (googleCalendarRoot.items == null)
            {
                return "none";
            }

            string calendarId = "";
            if (valueType == EventValueType.Summary)
            {
                foreach (var item in googleCalendarRoot.items)
                {
                    if (item.summary == val.ToString())
                    {
                        calendarId = item.id;
                        break;
                    }
                }
            }
            else if (valueType == EventValueType.Description)
            {
                foreach (var item in googleCalendarRoot.items)
                {
                    if (item.description == val.ToString())
                    {
                        calendarId = item.id;
                        break;
                    }
                }
            }
            else if (valueType == EventValueType.Location)
            {
                foreach (var item in googleCalendarRoot.items)
                {
                    if (item.location == val.ToString())
                    {
                        calendarId = item.id;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(calendarId))
            {
                return "none";
            }
            else
            {
                return calendarId;
            }

        }

        /// <summary>
        /// Format datetime to make proper google api calls.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public string GetProperDateTimeFormat(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        #endregion
    }
}
