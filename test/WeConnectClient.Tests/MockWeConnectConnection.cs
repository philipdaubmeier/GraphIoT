using PhilipDaubmeier.WeConnectClient.Network;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockWeConnectConnection
    {
        private const string _baseUri = "https://identity.vwgroup.io";
        private const string _portalUri = "https://www.portal.volkswagen-we.com";
        private const string _redirectUri = "https%3A%2F%2Fwww.portal.volkswagen-we.com%2Fportal%2Fweb%2Fguest%2Fcomplete-login";

        private const string _clientId = "b2345678-d123-4abc-9cde-89abcdef1234@apps_vw-dilab_com";
        private const string _userId = "b12341c9-d123-4abc-9cde-89abcdef1234";
        private const string _nonce = "c6677889-346b-5678-5432-b1234a3465bc";
        private const string _scopes = "openid%20profile%20birthdate%20nickname%20address%20phone%20cars%20mbb";
        private const string _relaystate = "0f9d7eaae_unittest_relaystate_1980e977e5";
        private const string _hmac2 = "8e208cd992bd_unittest_hmac2_f9f218793ed78945be3f232b4180129cf9bf";
        private const string _jwtToken = "eyJraWQiOiI2YTE1NzUyMTY3OGY3N2JhIiwiYWxnIjoiUlMyNTYifQ.ewogICJzdWIiOiAiYjEyMzQxYzktZDEyMy00YWJjLTljZGUtODlhYmNkZWYxMjM0IiwKICAiYXVkIjogImIyMzQ1Njc4LWQxMjMtNGFiYy05Y2RlLTg5YWJjZGVmMTIzNEBhcHBzX3Z3LWRpbGFiX2NvbSIsCiAgImFjciI6ICJodHRwczovL2lkZW50aXR5LnZ3Z3JvdXAuaW8vYXNzdXJhbmNlL2xvYS0yIiwKICAic2NwIjogIm9wZW5pZCBwcm9maWxlIGJpcnRoZGF0ZSBuaWNrbmFtZSBhZGRyZXNzIHBob25lIGNhcnMgbWJiIiwKICAiYWF0IjogImlkZW50aXR5a2l0IiwKICAiaXNzIjogImh0dHBzOi8vaWRlbnRpdHkudndncm91cC5pbyIsCiAgImp0dCI6ICJhdXRob3JpemF0aW9uX2NvZGUiLAogICJleHAiOiAxNTg4MDIwOTk4LAogICJpYXQiOiAxNTg4MDIwNjk4LAogICJub25jZSI6ICJjNjU0ODg5Yi0wZjliLTRiYTgtYTkzNi1iZDc0ODFkMjY4NmIiLAogICJqdGkiOiAiN2M4ZTBjNmYtMDc1MC00OGQ1LWIzNzAtNDVjYzQ1YTI5MWY4Igp9.PtzseyO4M3Qnsgj9WaQopUikn4yfDktkhAQ2tvVenkeqXtwPuFi8JA0-xDTPd2tU1DhVmuMTAeXRSS0Y8uaWfxfh8DBe19vPvY5I5zsFwQ6GRFYJ_UZ6MLwPvJniSa2ov1nRo4fO7fVZD2nHXsmM7bHa73f394YT_WhDQ7hGbPMBUsCK77H2KtwCeHtROWQV4rdlbRZ21TIL4A5YROw8bvWmpclwSC8UNUqweiDpZDfxSMiiwjpuOiwWRHPap4pTrsPOyUXRYj7yNg9sEnu88wMOVoaO7L6upV0oBLi_Iz2gZFhvGJnV38wmhB1gf2XEHQNPzl68uTs_J15MQGKRhwWYqtrwT9Gj46PefbQnhfWfBLPBsrxT5nAG74W-0-I0T_CNP87oq3CNV-emxMMR_qE5hH_FVjH0fgjlkXl_sgFmcRNpoGGejoP73Digv73VnRVHba50oaU-pGtxBkXCyiAZkTQFg6vA8g1RmBFG0Jplt2OxshQrC1IGkbxNuQPwLZaTwsxUciNkMcYWdzy3j-4urYeXBLPDmF46ju5rzWHfBhifxg43l8fZ8KOpDPTdkkrIyNeGOLoF-Rw4KPh5fdTjJlO4zpOxHJC-fXK9wCc0riPpYJrLjfjEyh9tNHlbrXwdnuZm4Q-HusB_trb7oE9ys3jLBlbRNFQ0ZmyLnKU";

        public static string Vin => "WVWZZZABCD1234567";
        public static string BaseUri => $"{_portalUri}/portal/delegate/dashboard/{Vin}";

        public class WeConnectMockConnectionProvider : WeConnectConnectionProvider
        {
            public WeConnectMockConnectionProvider(IWeConnectAuth authData, HttpClient mockClient, HttpClient mockAuthClient)
                : base(authData)
            {
                Client = mockClient;
                AuthClient = mockAuthClient;
            }
        }

        private static readonly IWeConnectAuth auth = new WeConnectAuth("john@doe.com", "secretpassword");

        public static WeConnectConnectionProvider ToMockProvider(this MockHttpMessageHandler mockHandler)
        {
            return new WeConnectMockConnectionProvider(auth, new HttpClient(mockHandler), new HttpClient(mockHandler));
        }

        public static MockHttpMessageHandler AddAuthMock(this MockHttpMessageHandler mockHttp)
        {
            // Step 1
            mockHttp.When($"{_portalUri}/portal/en_GB/web/guest/home")
                    .Respond("text/html",
                    @"<html>
                    <head>
                        <title>We Connect</title>
                        <meta name=""_csrf"" content=""UrWEaLPN""/>
                    </head>
                    <body></body>
                    </html>");

            // Step 2
            mockHttp.When($"{_portalUri}/portal/en_GB/web/guest/home/-/csrftokenhandling/get-login-url")
                    .Respond("application/json",
                    $"{{\"errorCode\":\"0\",\"loginURL\":{{\"path\":\"{_baseUri}/oidc/v1/authorize?ui_locales=en&scope={_scopes}&response_type=code&state=UrWEaLPN&redirect_uri={_redirectUri}&nonce={_nonce}&prompt=login&client_id={_clientId}\"}}}}");

            // Step 3
            mockHttp.When($"{_baseUri}/oidc/v1/authorize")
                    .WithExactQueryString($"ui_locales=en&scope={_scopes}&response_type=code&state=agkWdVBw&redirect_uri={_redirectUri}&nonce={_nonce}&prompt=login&client_id={_clientId}")
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("Location", $"{_baseUri}/signin-service/v1/signin/{_clientId}?relayState={_relaystate}")
                    }, "text/plain", "");

            // Step 4
            mockHttp.When($"{_baseUri}/signin-service/v1/signin/{_clientId}?relayState={_relaystate}")
                    .Respond("text/html",
                    @"<!DOCTYPE html>
                    <html>
                    <body>
                        <form method=""POST"" action=""/signin-service/v1/{_clientId}/login/identifier"">
                            <input type=""hidden"" id=""hmac"" name=""hmac"" value=""" + _hmac2 + @"""/>
                            <input type=""hidden"" id=""csrf"" name=""_csrf"" value=""ce777776-csrf-csrf-csrf-67555555f55a""/>
	                    </form>
                    </body>
                    </html>");

            // Step 5
            mockHttp.When($"{_baseUri}/signin-service/v1/{_clientId}/login/identifier")
                    .Respond("text/html",
                    @"<!DOCTYPE html>
                    <html>
                    <body>
                        <form method=""POST"" action=""/signin-service/v1/" + _clientId + @"/login/authenticate"">
                            <input type=""hidden"" id=""hmac"" name=""hmac"" value=""" + _hmac2 + @""" />
	                    </form>
                    </body>
                    </html>");

            // Step 6
            mockHttp.When($"{_baseUri}/signin-service/v1/{_clientId}/login/authenticate")
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("Location", $"{_baseUri}/oidc/v1/oauth/sso?clientId={_clientId}&relayState={_relaystate}&userId={_userId}&HMAC={_hmac2}")
                    }, "text/plain", "");

            mockHttp.When($"{_baseUri}/oidc/v1/oauth/sso")
                    .WithExactQueryString($"clientId={_clientId}&relayState={_relaystate}&userId={_userId}&HMAC={_hmac2}")
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("Location", $"{_baseUri}/signin-service/v1/consent/users/{_userId}/{_clientId}?scopes={_scopes}&relayState={_relaystate}&callback={_baseUri}/oidc/v1/oauth/client/callback&hmac={_hmac2}")
                    }, "text/plain", "");

            mockHttp.When($"{_baseUri}/signin-service/v1/consent/users/{_userId}/{_clientId}")
                    .WithExactQueryString($"scopes={_scopes}&relayState={_relaystate}&callback={_baseUri}/oidc/v1/oauth/client/callback&hmac={_hmac2}")
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("Location", $"{_baseUri}/oidc/v1/oauth/client/callback/success?user_id={_userId}&client_id={_clientId}&scopes={_scopes}&consentedScopes={_scopes}&relayState={_relaystate}&hmac={_hmac2}")
                    }, "text/plain", "");

            mockHttp.When($"{_baseUri}/oidc/v1/oauth/client/callback/success")
                    .WithExactQueryString($"user_id={_userId}&client_id={_clientId}&scopes={_scopes}&consentedScopes={_scopes}&relayState={_relaystate}&hmac={_hmac2}")
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("Location", $"{_portalUri}/portal/web/guest/complete-login?state=agkWdVBw&code={_jwtToken}")
                    }, "text/plain", "");

            // Step 7
            mockHttp.When($"{_portalUri}/portal/web/guest/complete-login")
                    .WithExactQueryString("p_auth=agkWdVBw&p_p_id=33_WAR_cored5portlet&p_p_lifecycle=1&p_p_state=normal&p_p_mode=view&p_p_col_id=column-1&p_p_col_count=1&_33_WAR_cored5portlet_javax.portlet.action=getLoginStatus")
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("Location", $"{_portalUri}/portal/delegate/dashboard/{Vin}")
                    }, "text/plain", "");

            // Step 8
            mockHttp.When($"{_portalUri}/portal/delegate/dashboard/{Vin}")
                    .Respond("text/html",
                    @"<!DOCTYPE html>
                    <html>
                    <head>
                        <title>We Connect</title>
                        <meta name=""_csrf"" content=""agkWdVBw""/>
                    </head>
                    <body></body>
                    </html>");

            return mockHttp;
        }
    }
}