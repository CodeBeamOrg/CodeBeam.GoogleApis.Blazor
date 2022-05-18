﻿using GoogleApis.Blazor.Auth;
using GoogleApis.Blazor.Extensions;
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
        [Inject] AuthService AuthService { get; set; }

        /// <summary>
        /// Constructs the class.
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="authService"></param>
        public CalendarService(IHttpClientFactory httpClientFactory, AuthService authService)
        {
            HttpClientFactory = httpClientFactory;
            AuthService = authService;
        }

        private string _accessToken = "";
        private string _refreshToken = "";

        /// <summary>
        /// The access token using throughout this service.
        /// </summary>
        public string AccessToken 
        {
            get => _accessToken;
            set
            {
                if (_accessToken != value)
                {
                    return;
                }
                _accessToken = value;
                AccessTokenChanged.InvokeAsync().AndForget();
            }
        }

        /// <summary>
        /// The refresh token using throughout this service.
        /// </summary>
        public string RefreshToken
        {
            get => _refreshToken;
            set
            {
                if (_refreshToken != value)
                {
                    return;
                }
                _refreshToken = value;
                RefreshTokenChanged.InvokeAsync().AndForget();
            }
        }

        /// <summary>
        /// An event calls when access token changed.
        /// </summary>
        public EventCallback AccessTokenChanged { get; set; }

        /// <summary>
        /// An event calls when refresh token changed.
        /// </summary>
        public EventCallback RefreshTokenChanged { get; set; }

        #region Calendars

        /// <summary>
        /// Get all calendars that authenticated user has. Returns max of 250 calendars.
        /// </summary>
        /// <returns></returns>
        public string GetCalendars(int maxResults = 250, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            //This uses calendar list instead of calendar. Read for the difference https://developers.google.com/calendar/api/concepts/events-calendars#calendar_and_calendar_list
            var result = client.GetAsync($"https://www.googleapis.com/calendar/v3/users/me/calendarList?maxResults={maxResults.ToString()}&access_token={_accessToken}").Result;

            string contentResult = result.Content.ReadAsStringAsync().Result;

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = AuthService.RefreshAccessToken(_refreshToken);
            GetCalendars();
            return "";
        }

        /// <summary>
        /// Get calendar with given id that authenticated user has.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string GetCalendarById(string calendarId, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            var result = client.GetAsync($"https://www.googleapis.com/calendar/v3/users/me/calendarList/{calendarId}?access_token=" + _accessToken).Result;

            string contentResult = result.Content.ReadAsStringAsync().Result;

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = AuthService.RefreshAccessToken(_refreshToken);
            GetCalendarById(calendarId);
            return "";
        }

        /// <summary>
        /// Get calendar metadata with given summary.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string GetCalendarBySummary(string summary, bool forceAccessToken = false)
        {
            string calendars = GetCalendars();
            GoogleCalendarListRoot jsonCalendar = JsonSerializer.Deserialize<GoogleCalendarListRoot>(calendars);

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

            return GetCalendarById(calendarId, forceAccessToken);
        }

        /// <summary>
        /// Creates a new secondary calendar into user's CalendarList.
        /// </summary>
        /// <param name="googleCalendarListModel"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns>Returns the request's result content.</returns>
        public string AddCalendar(GoogleCalendarListModel googleCalendarListModel, bool forceAccessToken = false)
        {
            string requestBody = JsonSerializer.Serialize(googleCalendarListModel);

            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var result = client.PostAsync($"https://www.googleapis.com/calendar/v3/calendars", content).Result;

            string contentResult = result.Content.ReadAsStringAsync().Result;

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = AuthService.RefreshAccessToken(_refreshToken);
            AddCalendar(googleCalendarListModel);
            return "";
        }

        /// <summary>
        /// Updates the specified calendar with given model.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="googleCalendarListModel"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string UpdateCalendar(string calendarId, GoogleCalendarListModel googleCalendarListModel, bool forceAccessToken = false)
        {
            string requestBody = JsonSerializer.Serialize(googleCalendarListModel);

            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var result = client.PutAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}", content).Result;

            string contentResult = result.Content.ReadAsStringAsync().Result;

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = AuthService.RefreshAccessToken(_refreshToken);
            UpdateCalendar(calendarId, googleCalendarListModel);
            return "";
        }

        /// <summary>
        /// Deletes the specified calendar. Only deletes the secondary calendars. Use Clear method for primary calendar.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string DeleteCalendar(string calendarId, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var result = client.DeleteAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}").Result;

            string contentResult = result.Content.ReadAsStringAsync().Result;

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = AuthService.RefreshAccessToken(_refreshToken);
            DeleteCalendar(calendarId);
            return "";
        }

        /// <summary>
        /// Deletes all events in a primary calendar. If it's a secondary calendar, it has no effect. Use DeleteCalendar for secondary ones.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string ClearCalendar(string calendarId, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var result = client.PostAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/clear", content).Result;

            string contentResult = result.Content.ReadAsStringAsync().Result;

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = AuthService.RefreshAccessToken(_refreshToken);
            ClearCalendar(calendarId);
            return "";
        }

        #endregion


        #region Events

        /// <summary>
        /// Get event list within a specific calendar between the specified dates. Returns a max of 2500 events.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="timeMin"></param>
        /// <param name="timeMax"></param>
        /// <param name="maxResults">Select how many items to return. Max is 2500.</param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string GetEvents(DateTime timeMin, DateTime timeMax, string calendarId, int maxResults = 2500, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            var result = client.GetAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events?access_token={_accessToken}&maxResults={maxResults.ToString()}").Result;

            string contentResult = result.Content.ReadAsStringAsync().Result;

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = AuthService.RefreshAccessToken(_refreshToken);
            GetEvents(timeMin, timeMax, calendarId, maxResults);
            return "";
        }

        /// <summary>
        /// Get event in a specific calendar with a given id.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="eventId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string GetEventById(string eventId, string calendarId, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            var result = client.GetAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events/{eventId}?access_token=" + _accessToken).Result;

            string contentResult = result.Content.ReadAsStringAsync().Result;

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = AuthService.RefreshAccessToken(_refreshToken);
            GetEventById(eventId, calendarId);
            return "";
        }

        /// <summary>
        /// Creates a new event in a specific calendar.
        /// </summary>
        /// <param name="calendarEvent"></param>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string AddEvent(GoogleCalendarEventModel calendarEvent, string calendarId, bool forceAccessToken = false)
        {
            string requestBody = JsonSerializer.Serialize(calendarEvent);

            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = client.PostAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events", content).Result;

            string contentResult = result.Content.ReadAsStringAsync().Result;

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = AuthService.RefreshAccessToken(_refreshToken);
            AddEvent(calendarEvent, calendarId);
            return "";
        }

        /// <summary>
        /// Updates the event.
        /// </summary>
        /// <param name="newCalendarEvent"></param>
        /// <param name="eventId"></param>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string UpdateEvent(GoogleCalendarEventModel newCalendarEvent, string eventId, string calendarId, bool forceAccessToken = false)
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

            string contentResult = result.Content.ReadAsStringAsync().Result;

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = AuthService.RefreshAccessToken(_refreshToken);
            UpdateEvent(newCalendarEvent, eventId, calendarId);
            return "";
        }

        /// <summary>
        /// Finds event in specified calendar with given min and max time period and the summary.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string GetEventBySummary(string summary, DateTime dateMin, DateTime dateMax, string calendarId, bool forceAccessToken = false)
        {
            string events = GetEvents(dateMin, dateMax, calendarId);
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

            return GetEventById(eventId, calendarId, forceAccessToken);
        }

        /// <summary>
        /// Finds the event with given summary in given GoogleCalendarEventRoot.items collection.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="googleCalendarEventRoot"></param>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string GetEventBySummary(string summary, GoogleCalendarEventRoot googleCalendarEventRoot, string calendarId, bool forceAccessToken = false)
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

            return GetEventById(eventId, calendarId, forceAccessToken);
        }

        /// <summary>
        /// Finds event in specified calendar with given min and max time period and the description.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string GetEventByDescription(string description, DateTime dateMin, DateTime dateMax, string calendarId, bool forceAccessToken = false)
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

            return GetEventById(eventId, calendarId, forceAccessToken);
        }

        /// <summary>
        /// Finds the event with given description in given GoogleCalendarEventRoot.items collection.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="googleCalendarEventRoot"></param>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string GetEventByDescription(string description, GoogleCalendarEventRoot googleCalendarEventRoot, string calendarId, bool forceAccessToken = false)
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

            return GetEventById(eventId, calendarId, forceAccessToken);
        }

        /// <summary>
        /// Deletes the event in a specific calendar.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public string DeleteEvent(string eventId, string calendarId, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var result = client.DeleteAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events/{eventId}").Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            string contentResult = result.Content.ReadAsStringAsync().Result;

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = AuthService.RefreshAccessToken(_refreshToken);
            DeleteEvent(eventId, calendarId);
            return "";
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
            string calendars = GetCalendars();
            if (calendars == "error")
            {
                return "error: Can't fetch calendars.";
            }
            GoogleCalendarListRoot googleCalendarRoot = JsonSerializer.Deserialize<GoogleCalendarListRoot>(calendars);

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
        /// Get the primary calendar in calendar list. Returns null if not found.
        /// </summary>
        /// <returns></returns>
        public GoogleCalendarListModel FindPrimaryCalendar()
        {
            GoogleCalendarListRoot calendarList = JsonSerializer.Deserialize<GoogleCalendarListRoot>(GetCalendars());
            foreach (GoogleCalendarListModel item in calendarList.items ?? new List<GoogleCalendarListModel>())
            {
                if (item.primary == true)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds and returns the event id which matches with the given value and type.
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="val"></param>
        /// <param name="timeMin"></param>
        /// <param name="timeMax"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public string FindEventId(EventValueType valueType, object val, DateTime timeMin, DateTime timeMax, string calendarId)
        {
            string googleEvents = GetEvents(timeMin, timeMax, calendarId);
            GoogleCalendarEventRoot googleCalendarEventRoot = JsonSerializer.Deserialize<GoogleCalendarEventRoot>(googleEvents);

            if (googleCalendarEventRoot.items == null)
            {
                return "none";
            }

            string eventId = "";
            if (valueType == EventValueType.Summary)
            {
                foreach (var item in googleCalendarEventRoot.items)
                {
                    if (item.summary == val.ToString())
                    {
                        eventId = item.id;
                        break;
                    }
                }
            }
            else if (valueType == EventValueType.Description)
            {
                foreach (var item in googleCalendarEventRoot.items)
                {
                    if (item.description == val.ToString())
                    {
                        eventId = item.id;
                        break;
                    }
                }
            }
            else if (valueType == EventValueType.Location)
            {
                foreach (var item in googleCalendarEventRoot.items)
                {
                    if (item.location == val.ToString())
                    {
                        eventId = item.id;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(eventId))
            {
                return "none";
            }
            else
            {
                return eventId;
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
