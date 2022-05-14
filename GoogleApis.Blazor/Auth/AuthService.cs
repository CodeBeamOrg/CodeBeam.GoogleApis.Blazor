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
    /// <summary>
    /// A class about Google OAuth2 and some user process.
    /// </summary>
    public class AuthService
    {
        [Inject] IHttpClientFactory HttpClientFactory { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        
        /// <summary>
        /// Constructs the class.
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="httpClientFactory"></param>
        public AuthService(IJSRuntime jsRuntime, IHttpClientFactory httpClientFactory)
        {
            JSRuntime = jsRuntime;
            HttpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// oAuth2 Step 1. Opens "Select Google Account" page in a new tab and return the value into redirectUrl.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="scopes"></param>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        public async Task RequestAuthorizationCode(string clientId, List<Scope> scopes, string redirectUrl)
        {
            string encodedRedirectUrl = HttpUtility.UrlEncode(redirectUrl);
            if (scopes == null || scopes.Count == 0)
            {
                return;
            }
            List<string> scopeStringList = new List<string>();
            foreach (var scope in scopes)
            {
                scopeStringList.Add(scope.ToDescriptionString());
            }
            string scopeString = string.Join("+", scopeStringList);

            await JSRuntime.InvokeAsync<object>("open", $"https://accounts.google.com/o/oauth2/auth/oauthchooseaccount?response_type=code&client_id={clientId}&scope={scopeString}&redirect_uri={encodedRedirectUrl}&flowName=GeneralOAuthFlow&access_type=offline&prompt=consent", "_blank");
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

        /// <summary>
        /// Get authenticated user's e-mail adress.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public string GetUserMail(string accessToken)
        {
            var client = HttpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var result = client.GetAsync("https://www.googleapis.com/userinfo/v2/me").Result;

            var jsonResult = JsonDocument.Parse(result.Content.ReadAsStringAsync().Result);
            string email = jsonResult.RootElement.GetProperty("email").ToString();

            return email;
        }

        /// <summary>
        /// Brings a value into a given credential string. The string can be obtain with result of AuthorizeCredential method.
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="credentialValueType"></param>
        /// <returns></returns>
        public string GetValueFromCredential(string credential, CredentialValueType credentialValueType)
        {
            string accessToken = string.Empty;

            var jsonResult = JsonDocument.Parse(credential);
            accessToken = jsonResult.RootElement.GetProperty(credentialValueType.ToDescriptionString()).ToString();

            return accessToken;
        }

        /// <summary>
        /// Get information with given accesstoken like scope, expires in etc. Returns a json format.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public string GetAccessTokenDetails(string accessToken)
        {
            var client = HttpClientFactory.CreateClient();
            var result = client.GetAsync($"https://oauth2.googleapis.com/tokeninfo?access_token={accessToken}").Result;

            return result.Content.ReadAsStringAsync().Result;
        }

    }
}
