#pragma warning disable CS1591
using System.ComponentModel;

namespace GoogleApis.Blazor.Extensions
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation,
        /// using the specified key/value string pairs (required) as query strings. 
        /// <example>
        /// For example:
        /// <code>
        /// var result = await httpClient.GetWithQueryStringsAsync(uri, new[] {
        ///     "timeMin", GetProperDateTimeFormat(timeMin),
        ///     "timeMax", GetProperDateTimeFormat(timeMax),
        ///     "timeZone", timeZone // might be a null value
        /// });
        /// </code>
        /// results in <c>p</c>'s having the value (2,8).
        /// </example>        
        /// </summary>
        /// <param name="client"></param>        
        /// <param name="requestUri">The Uri the request is sent to.</param>        
        /// <param name="queryParameters">
        ///     String array representing a set of key/value pairs to append the 
        ///     <paramref>requestUri</paramref> as query strings elements.
        ///     
        ///     For each pair, the first element is a "key" and second is its "value".
        ///     Null keys are not permitted and will result in a runtime 
        ///     <typeparamref>ArgumentNullException</typeparamref>. Null values are
        ///     permitted, and the pair as a whole will be ignored.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>    
        /// <exception cref="System.ArgumentNullException">
        ///     The <paramref>requestUri</paramref> parameter is required.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The <paramref>requestUri</paramref> parameter is assumed to be an <c>String</c>
        ///     array representing key/value pairs, and therefore must be an even number in total.
        ///     Null values are prohibited with the key, but allowed with the value.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     The <paramref>requestUri</paramref> parameter is assumed to be an <c>String</c>
        ///     array representing key/value pairs, and therefore must be an even number in total.
        ///     Null values are prohibited with the key, but allowed with the value.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        ///     must be set.
        /// </exception>
        /// <exception cref="System.Net.Http.HttpRequestException">
        ///     The request failed due to an underlying issue such as network connectivity, DNS
        ///     failure, server certificate validation or timeout.
        /// </exception>
        /// <exception cref="System.Threading.Tasks.TaskCanceledException">
        ///     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        /// </exception>
        public static async Task<HttpResponseMessage> GetWithQueryStringsAsync(this HttpClient client, string requestUri, params string[] queryParameters)
        {
            if (queryParameters == null || queryParameters.Length == 0) {
                throw new ArgumentNullException(nameof(queryParameters));
            }
            if (queryParameters.Length % 2 != 0) { // Isn't even?
                throw new ArgumentOutOfRangeException(nameof(queryParameters), "queryParameters is expected to be a set of key/value string pairs, and therefore should be an even number of strings.");
            }

            var uri = requestUri;
            string Separator() => uri.Contains('?') ? "&" : "?";

            var numberPairs = queryParameters.Length / 2;
            for (var i = 0; i < numberPairs; i++)
            {
                var key = queryParameters[2 * i];
                var value = queryParameters[2 * i + 1];

                if (key == null) 
                    throw new ArgumentException("For each pair taken from the queryParameters string array, the first item is treated as a \"key\" and is therefore required.", nameof(queryParameters));
                if (value == null) continue;

                uri += $"{Separator()}{key}={Uri.EscapeDataString(value)}";   
            }
            
            return await client.GetAsync(uri);
        }
    }
}
