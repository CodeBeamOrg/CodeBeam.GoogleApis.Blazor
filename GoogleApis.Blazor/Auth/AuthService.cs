using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace GoogleApis.Blazor.Auth
{
    public class AuthService
    {
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IHttpClientFactory HttpClientFactory { get; set; }

        public AuthService(IJSRuntime jsRuntime, IHttpClientFactory httpClientFactory)
        {
            JSRuntime = jsRuntime;
            HttpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// oAuth2 Step 1. Opens "Select Google Account" page in a new tab and return the value into redirectUrl.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        public async Task RequestAuthorizationCode(string clientId, Scope scope, string redirectUrl)
        {
            string encodedRedirectUrl = HttpUtility.UrlEncode(redirectUrl);
            await JSRuntime.InvokeAsync<object>("open", $"https://accounts.google.com/o/oauth2/auth/oauthchooseaccount?response_type=code&client_id={clientId}&scope={scope.ToDescriptionString()}&redirect_uri={encodedRedirectUrl}&flowName=GeneralOAuthFlow&access_type=offline&prompt=consent", "_blank");

        }

        /// <summary>
        /// oAuth2 Step 2. Get credentials with given authorizationCode. Returns string content.
        /// </summary>
        /// <param name="authorizationCode"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        public string AuthorizeCredential(string authorizationCode, string clientId, string clientSecret, string redirectUrl)
        {
            string encodedAuthorizationCode = HttpUtility.UrlEncode(authorizationCode);
            string encodedRedirectUrl = HttpUtility.UrlEncode(redirectUrl);

            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var content = new StringContent($"code={encodedAuthorizationCode}&client_id={clientId}&client_secret={clientSecret}&redirect_uri={encodedRedirectUrl}&scope=&grant_type=authorization_code", Encoding.UTF8, "application/x-www-form-urlencoded");

            var request = client.PostAsync("https://oauth2.googleapis.com/token", content).Result;

            return request.Content.ReadAsStringAsync().Result;
        }

        public string GetValueFromCredential(string credential, CredentialValueType credentialValueType)
        {
            string accessToken = string.Empty;

            var jsonResult = JsonDocument.Parse(credential);
            accessToken = jsonResult.RootElement.GetProperty(credentialValueType.ToDescriptionString()).ToString();

            return accessToken;
        }

    }
}
