using PhilipDaubmeier.SonnenClient.Network;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Http;
using System;

namespace PhilipDaubmeier.SonnenClient.Tests
{
    public static class MockSonnenConnection
    {
        private const string _clientId = "1234561unittestidf91d15ff4caceee";
        private const string _authenticityToken = "55PRAC+bGdpy0_unittest_authenticity_token_eRsMi8LOsCCnZa45ac0YEwelytaIlwNW60WljrfOkpmw==";
        private const string _authorizationCode = "123_unittest_authorization_code_456";

        private const string _username = "john@doe.com";
        private const string _password = "secretpassword";

        public static string BaseUri => "https://my-api.sonnen.de/v1/";
        public static string AppToken => "5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2";
        public static string RefreshToken => "716289babc_dummy_unittest_refresh_token_83025a07162890587bea58987be";
        public static string SiteId => "3b8914b2-1934-4376-a593-ddfb0226eca5";

        private static readonly ISonnenAuth auth = new SonnenAuth(_username, _password);

        public class SonnenMockConnectionProvider : SonnenConnectionProvider
        {
            public SonnenMockConnectionProvider(ISonnenAuth authData, string clientId, HttpClient mockClient)
                : base(authData, clientId)
            {
                Client = mockClient;
            }
        }

        public static SonnenConnectionProvider ToMockProvider(this MockHttpMessageHandler mockHandler)
        {
            return new SonnenMockConnectionProvider(auth, _clientId, mockHandler.ToHttpClient());
        }

        public static MockHttpMessageHandler AddAuthMock(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When("https://account.sonnen.de/oauth/authorize").WithQueryString(new[] {
                        ("client_id", _clientId),
                        ("response_type", "code"),
                        ("redirect_uri", "https://my.sonnen.de/"),
                        ("code_challenge_method", "S256"),
                        ("locale", "de")
                    }.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)))
                    .Respond("text/html",
                    @"<!DOCTYPE html>
                    <html><head><title>sonnenID</title></head>
                    <body>
                        <main>
                            <div class=""card card-sonnen card-width-25s"">
                                <div class=""card-body text-center"">
                                    <p class=""lead"">Welcome to sonnen</p>
                                    <form class=""simple_form new_user"" id=""new_user"" novalidate=""novalidate"" action=""/users/sign_in"" accept-charset=""UTF-8"" method=""post"">
                                        <input type=""hidden"" name=""authenticity_token"" value=""" + _authenticityToken + @""" />
                                        <input class=""form-control string email optional"" autocomplete=""email"" placeholder=""E-Mail"" type=""email"" name=""user[email]"" id=""user_email"" />
                                        <input class=""form-control password optional"" autocomplete=""current-password"" placeholder=""Passwort"" type=""password"" name=""user[password]"" id=""user_password"" />
                                        <fieldset class=""form-group boolean optional user_remember_me mb-4 text-left"">
                                            <input name=""user[remember_me]"" type=""hidden"" value=""0"" />
                                            <input class=""form-check-input boolean optional"" type=""checkbox"" value=""1"" name=""user[remember_me]"" id=""user_remember_me"" />
                                            <label class=""form-check-label boolean optional"" for=""user_remember_me"">stay logged in</label>
                                        </fieldset>
                                        <div class=""submit-wrapper mb-5"">
                                            <input type=""submit"" name=""commit"" value=""Log in"" class=""btn btn btn-primary btn-sign-in sonnen-id-submit"" data-disable-with=""Sign in"" />
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </main>
                    </body></html>");

            mockHttp.When($"https://account.sonnen.de/users/sign_in")
                    .WithFormData(new[]
                    {
                        ("authenticity_token", _authenticityToken),
                        ("user[email]", _username),
                        ("user[password]", _password),
                        ("user[remember_me]", "0"),
                        ("commit", "Log+in")
                    }.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)))
                    .Respond(req =>
                    {
                        req.RequestUri = new Uri($"https://my.sonnen.de/?code={_authorizationCode}");
                        return new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = req };
                    });

            mockHttp.When($"https://account.sonnen.de/oauth/token")
                    .WithFormData(new[]
                    {
                        ("grant_type", "authorization_code"),
                        ("code", _authorizationCode),
                        ("client_id", _clientId),
                        ("redirect_uri", "https://my.sonnen.de/")
                    }.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)))
                    .Respond("application/json",
                    @"{

                        ""access_token"": """ + AppToken + @""",
                        ""token_type"": """",
                        ""expires_in"": 36000,
                        ""refresh_token"": """ + RefreshToken + @""",
                        ""created_at"": " + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + @"
                    }");

            mockHttp.When($"https://account.sonnen.de/oauth/token")
                    .WithExactFormData(new[]
                    {
                        ("client_id", _clientId),
                        ("refresh_token", RefreshToken),
                        ("grant_type", "refresh_token")
                    }.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)))
                    .Respond("application/json",
                    @"{

                        ""access_token"": """ + AppToken + @""",
                        ""token_type"": """",
                        ""expires_in"": 36000,
                        ""refresh_token"": """ + RefreshToken + @""",
                        ""created_at"": " + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + @"
                    }");

            return mockHttp;
        }
    }
}