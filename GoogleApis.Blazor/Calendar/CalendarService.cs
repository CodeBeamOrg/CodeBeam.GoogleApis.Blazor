using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApis.Blazor.Calendar
{
    public class CalendarService
    {
        [Inject] IHttpClientFactory HttpClientFactory { get; set; }

        public CalendarService(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Get all calendars that authenticated user has.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public string GetCalendars(string accessToken)
        {
            var client = HttpClientFactory.CreateClient();

            var result = client.GetAsync("https://www.googleapis.com/calendar/v3/users/me/calendarList?access_token=" + accessToken).Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Get all calendars that authenticated user has.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public string GetCalendar(string accessToken, string calendarId)
        {
            var client = HttpClientFactory.CreateClient();

            var result = client.GetAsync($"https://www.googleapis.com/calendar/v3/users/me/calendarList/{calendarId}?access_token=" + accessToken).Result;

            if (!result.IsSuccessStatusCode)
            {
                return "error";
            }

            return result.Content.ReadAsStringAsync().Result;
        }

    }
}
