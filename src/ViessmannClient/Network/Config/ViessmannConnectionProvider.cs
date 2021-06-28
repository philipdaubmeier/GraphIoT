using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace PhilipDaubmeier.ViessmannClient.Network
{
    public class ViessmannConnectionProvider<T> : IViessmannConnectionProvider<T>, IDisposable
    {
        private bool skipDisposingClient = false;
        private HttpClient? client = null;

        /// <summary>
        /// See <see cref="IViessmannConnectionProvider{T}.AuthData"/>
        /// </summary>
        public IViessmannAuth AuthData { get; set; }

        /// <summary>
        /// See <see cref="IViessmannConnectionProvider{T}.Client"/>
        /// </summary>
        public HttpClient Client
        {
            get => client ??= new HttpClient(CreateHttpClientHandler());
            protected set
            {
                client = value;
                skipDisposingClient = true;
            }
        }

        public static HttpClientHandler CreateHttpClientHandler() => new();

        public string ClientId { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;

        private static readonly string _verifierAllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
        private static readonly int _verifierLength = 128;
        private static readonly char[] _base64Padding = { '=' };
        private static readonly string _codeVerifier = GenerateCodeVerifier();

        public string CodeVerifier => _codeVerifier;
        public string CodeChallenge => ToCodeChallenge(CodeVerifier);
        public string CodeChallengeMethod => "S256";

        public ViessmannConnectionProvider(IViessmannAuth authData)
            => AuthData = authData;

        private static string GenerateCodeVerifier()
        {
            var rand = new Random();
            var chars = new char[_verifierLength];
            for (var i = 0; i < _verifierLength; i++)
                chars[i] = _verifierAllowedChars[rand.Next(0, _verifierAllowedChars.Length)];
            return new string(chars);
        }

        /// <summary>
        /// Calculates base64url(SHA256(codeVerifier))
        /// </summary>
        private static string ToCodeChallenge(string codeVerifier)
        {
            using var sha256Hash = SHA256.Create();
            byte[] hashBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));

            return Convert.ToBase64String(hashBytes).TrimEnd(_base64Padding).Replace('+', '-').Replace('/', '_');
        }

        #region IDisposable Support
        private bool disposed = false;

        public void Dispose()
        {
            if (disposed)
                return;

            if (!skipDisposingClient)
                client?.Dispose();

            disposed = true;
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}