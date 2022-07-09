using GoogleApis.Blazor.Auth;
using GoogleApis.Blazor.Extensions;
using GoogleApis.Blazor.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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
                if (_accessToken == value)
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
                if (_refreshToken == value)
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
        public async Task<GoogleCalendarListRoot> GetCalendars(int maxResults = 250, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            //This uses calendar list instead of calendar. Read for the difference https://developers.google.com/calendar/api/concepts/events-calendars#calendar_and_calendar_list
            var result = await client.GetAsync($"https://www.googleapis.com/calendar/v3/users/me/calendarList?maxResults={maxResults.ToString()}&access_token={_accessToken}");

            string contentResult = await result.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<GoogleCalendarListRoot>(contentResult);

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return model;
            }

            AccessToken = await AuthService.RefreshAccessToken(_refreshToken);
            return await GetCalendars();
        }

        /// <summary>
        /// Get calendar with given id that authenticated user has.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public async Task<GoogleCalendarListModel> GetCalendarById(string calendarId, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            var result = await client.GetAsync($"https://www.googleapis.com/calendar/v3/users/me/calendarList/{calendarId}?access_token=" + _accessToken);

            string contentResult = await result.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<GoogleCalendarListModel>(contentResult);

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return model;
            }

            AccessToken = await AuthService.RefreshAccessToken(_refreshToken);
            return await GetCalendarById(calendarId);
        }

        /// <summary>
        /// Get calendar metadata with given summary.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public async Task<GoogleCalendarListModel> GetCalendarBySummary(string summary, bool forceAccessToken = false)
        {
            var calendars = await GetCalendars();

            if (calendars.items == null)
            {
                return null;
            }

            string calendarId = "";
            foreach (var item in calendars.items)
            {
                if (item.summary == summary)
                {
                    calendarId = item.id;
                    break;
                }
            }

            if (string.IsNullOrEmpty(calendarId))
            {
                return null;
            }

            return await GetCalendarById(calendarId, forceAccessToken);
        }

        /// <summary>
        /// Creates a new secondary calendar into user's CalendarList.
        /// </summary>
        /// <param name="googleCalendarListModel"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns>Returns the request's result content.</returns>
        public async Task<GoogleCalendarModel> AddCalendar(GoogleCalendarListModel googleCalendarListModel, bool forceAccessToken = false)
        {
            string requestBody = JsonSerializer.Serialize(googleCalendarListModel);

            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var result = await client.PostAsync($"https://www.googleapis.com/calendar/v3/calendars", content);

            string contentResult = await result.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<GoogleCalendarModel>(contentResult);

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return model;
            }

            AccessToken = await AuthService.RefreshAccessToken(_refreshToken);
            return await AddCalendar(googleCalendarListModel);
        }

        /// <summary>
        /// Updates the specified calendar with given model.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="googleCalendarListModel"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public async Task<GoogleCalendarModel> UpdateCalendar(string calendarId, GoogleCalendarListModel googleCalendarListModel, bool forceAccessToken = false)
        {
            string requestBody = JsonSerializer.Serialize(googleCalendarListModel);

            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var result = await client.PutAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}", content);

            string contentResult = await result.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<GoogleCalendarModel>(contentResult);

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return model;
            }

            AccessToken = await AuthService.RefreshAccessToken(_refreshToken);
            return await UpdateCalendar(calendarId, googleCalendarListModel);
        }

        /// <summary>
        /// Deletes the specified calendar. Only deletes the secondary calendars. Use Clear method for primary calendar.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public async Task DeleteCalendar(string calendarId, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var result = await client.DeleteAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}");

            string contentResult = await result.Content.ReadAsStringAsync();

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return;
            }

            AccessToken = await AuthService.RefreshAccessToken(_refreshToken);
            await DeleteCalendar(calendarId);
        }

        /// <summary>
        /// Deletes all events in a primary calendar. If it's a secondary calendar, it has no effect. Use DeleteCalendar for secondary ones.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public async Task ClearCalendar(string calendarId, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var result = await client.PostAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/clear", content);

            string contentResult = await result.Content.ReadAsStringAsync();

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return;
            }

            AccessToken = await AuthService.RefreshAccessToken(_refreshToken);
            await ClearCalendar(calendarId);
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
        /// <param name="timeZone">Time zone used in the response. Optional. The default is the time zone of the calendar.</param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public async Task<GoogleCalendarEventRoot> GetEvents(DateTime timeMin, DateTime timeMax, string calendarId, int maxResults = 2500, string timeZone = null, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            var uri = $"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events?access_token={_accessToken}&maxResults={maxResults.ToString()}";
            
            var result = await client.GetWithQueryStringsAsync(uri, new[] {
                "timeMin", GetProperDateTimeFormat(timeMin),
                "timeMax", GetProperDateTimeFormat(timeMax),
                "timeZone", timeZone
            });

            string contentResult = await result.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<GoogleCalendarEventRoot>(contentResult);

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return model;
            }

            AccessToken = await AuthService.RefreshAccessToken(_refreshToken);
            return await GetEvents(timeMin, timeMax, calendarId, maxResults);
        }

        /// <summary>
        /// Get event in a specific calendar with a given id.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="eventId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public async Task<GoogleCalendarEventModel> GetEventById(string eventId, string calendarId, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            var result = await client.GetAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events/{eventId}?access_token=" + _accessToken);

            string contentResult = await result.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<GoogleCalendarEventModel>(contentResult);

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return json;
            }

            AccessToken = await AuthService.RefreshAccessToken(_refreshToken);
            return await GetEventById(eventId, calendarId);
        }

        /// <summary>
        /// Creates a new event in a specific calendar.
        /// </summary>
        /// <param name="calendarEvent"></param>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public async Task<string> AddEvent(GoogleCalendarEventModel calendarEvent, string calendarId, bool forceAccessToken = false)
        {
            string requestBody = JsonSerializer.Serialize(calendarEvent);

            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var result = await client.PostAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events", content);

            string contentResult = await result.Content.ReadAsStringAsync();

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = await AuthService.RefreshAccessToken(_refreshToken);
            await AddEvent(calendarEvent, calendarId);
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
        public async Task<string> UpdateEvent(GoogleCalendarEventModel newCalendarEvent, string eventId, string calendarId, bool forceAccessToken = false)
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
            result = await client.PutAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events/{eventId}", content);

            string contentResult = await result.Content.ReadAsStringAsync();

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return contentResult;
            }

            AccessToken = await AuthService.RefreshAccessToken(_refreshToken);
            await UpdateEvent(newCalendarEvent, eventId, calendarId);
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
        public async Task<GoogleCalendarEventModel> GetEventBySummary(string summary, DateTime dateMin, DateTime dateMax, string calendarId, bool forceAccessToken = false)
        {
            var events = await GetEvents(dateMin, dateMax, calendarId);

            if (events.items == null)
            {
                return null;
            }

            string eventId = "";
            foreach (var item in events.items)
            {
                if (item.summary == summary)
                {
                    eventId = item.id;
                }
            }

            if (string.IsNullOrEmpty(eventId))
            {
                return null;
            }

            return await GetEventById(eventId, calendarId, forceAccessToken);
        }

        /// <summary>
        /// Finds the event with given summary in given GoogleCalendarEventRoot.items collection.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="googleCalendarEventRoot"></param>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public async Task<GoogleCalendarEventModel> GetEventBySummary(string summary, GoogleCalendarEventRoot googleCalendarEventRoot, string calendarId, bool forceAccessToken = false)
        {
            string eventId = "";
            if (googleCalendarEventRoot.items == null)
            {
                return null;
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
                return null;
            }

            return await GetEventById(eventId, calendarId, forceAccessToken);
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
        public async Task<GoogleCalendarEventModel> GetEventByDescription(string description, DateTime dateMin, DateTime dateMax, string calendarId, bool forceAccessToken = false)
        {
            var events = await GetEvents(dateMin, dateMax, calendarId);
            if (events == null)
            {
                return null;
            }

            if (events.items == null)
            {
                return null;
            }

            string eventId = "";
            foreach (var item in events.items)
            {
                if (item.description == description)
                {
                    eventId = item.id;
                }
            }

            if (string.IsNullOrEmpty(eventId))
            {
                return null;
            }

            return await GetEventById(eventId, calendarId, forceAccessToken);
        }

        /// <summary>
        /// Finds the event with given description in given GoogleCalendarEventRoot.items collection.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="googleCalendarEventRoot"></param>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public async Task<GoogleCalendarEventModel> GetEventByDescription(string description, GoogleCalendarEventRoot googleCalendarEventRoot, string calendarId, bool forceAccessToken = false)
        {
            string eventId = "";
            if (googleCalendarEventRoot.items == null)
            {
                return null;
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
                return null;
            }

            return await GetEventById(eventId, calendarId, forceAccessToken);
        }

        /// <summary>
        /// Deletes the event in a specific calendar.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="calendarId"></param>
        /// <param name="forceAccessToken">If true and access token expired, it automatically calls for new access token with refresh token.</param>
        /// <returns></returns>
        public async Task DeleteEvent(string eventId, string calendarId, bool forceAccessToken = false)
        {
            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var result = await client.DeleteAsync($"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events/{eventId}");

            if (!result.IsSuccessStatusCode)
            {
                return;
            }

            string contentResult = await result.Content.ReadAsStringAsync();

            if (forceAccessToken == false || !AuthService.IsAccessTokenExpired(contentResult))
            {
                return;
            }

            AccessToken = await AuthService.RefreshAccessToken(_refreshToken);
            await DeleteEvent(eventId, calendarId);
        }

        #endregion


        #region Utilities

        /// <summary>
        /// Finds and returns the calendar id which matches with the given value.
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<string> FindCalendarId(CalendarValueType valueType, object val)
        {
            var calendars = await GetCalendars();
            if (calendars == null)
            {
                return null;
            }

            if (calendars.items == null)
            {
                return "none";
            }

            string calendarId = "";
            if (valueType == CalendarValueType.Summary)
            {
                foreach (var item in calendars.items)
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
                foreach (var item in calendars.items)
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
                foreach (var item in calendars.items)
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
        public async Task<GoogleCalendarListModel> FindPrimaryCalendar()
        {
            var calendarList = await GetCalendars();
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
        public async Task<string> FindEventId(EventValueType valueType, object val, DateTime timeMin, DateTime timeMax, string calendarId)
        {
            var googleCalendarEventRoot = await GetEvents(timeMin, timeMax, calendarId);
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
