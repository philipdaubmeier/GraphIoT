using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PhilipDaubmeier.WeConnectClient.Model.Auth
{
    [DebuggerDisplay("scp: {string.Join(' ', Scopes),nq}; sub: {Subject,nq}; iss: {Issuer,nq}")]
    public record ReadableJwt
    {
        private readonly JwtHeader _header;
        private readonly JwtPayload _payload;

        public string JwtToken { get; init; }

        public string JwtKeyId => _header.Kid;
        public string JwtAlgorithm => _header.Alg;

        public string Subject => _payload.Sub;
        public string Audience => _payload.Aud;
        public IEnumerable<string> Scopes => _payload.Scp.Split(' ');
        public string ActingSubject => _payload.Act.Sub;
        public string ActingIssuer => _payload.Act.Iss;
        public string AuthServerType => _payload.Aat;
        public string Issuer => _payload.Iss;
        public string TokenType => _payload.Jtt;
        public DateTime Expiration => DateTimeOffset.FromUnixTimeSeconds(_payload.Exp).UtcDateTime;
        public DateTime IssuedAt => DateTimeOffset.FromUnixTimeSeconds(_payload.Iat).UtcDateTime;
        public List<string> LegalEntity { get; set; } = new List<string>();
        public string JwtTokenId => _payload.Jti;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ReadableJwt(string JwtToken)
        {
            this.JwtToken = JwtToken;

            var splitted = JwtToken.Split('.');
            var header = ExtractDecode(splitted, 0);
            var payload = ExtractDecode(splitted, 1);

            try
            {
                _header = JsonSerializer.Deserialize<JwtHeader>(header, _jsonSerializerOptions) ?? new();
                _payload = JsonSerializer.Deserialize<JwtPayload>(payload, _jsonSerializerOptions) ?? new();
            }
            catch
            {
                _header = new();
                _payload = new();
            }
        }

        private static string ExtractDecode(string[]? array, int index)
        {
            var extractedBase64UrlSafe = array is not null && array.Count() > index ? array[index] ?? string.Empty : string.Empty;
            try
            {
                var base64 = extractedBase64UrlSafe.Replace('-', '+').Replace('_', '/');
                base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
                return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            }
            catch
            {
                return string.Empty;
            }
        }

        public static implicit operator ReadableJwt(string token) => new(token);

        public static implicit operator string(ReadableJwt token) => token.JwtToken;

        public override string ToString() => this;
    }

    internal class JwtHeader
    {
        public string Kid { get; set; } = string.Empty;
        public string Alg { get; set; } = string.Empty;
    }

    internal class JwtPayload
    {
        public string Sub { get; set; } = string.Empty;
        public string Aud { get; set; } = string.Empty;
        public string Scp { get; set; } = string.Empty;
        public JwtPayloadAct Act { get; set; } = new();
        public string Aat { get; set; } = string.Empty;
        public string Iss { get; set; } = string.Empty;
        public string Jtt { get; set; } = string.Empty;
        public long Exp { get; set; } = 0;
        public long Iat { get; set; } = 0;
        public List<string> Lee { get; set; } = new List<string>();
        public string Jti { get; set; } = string.Empty;
    }

    internal class JwtPayloadAct
    {
        public string Sub { get; set; } = string.Empty;
        public string Iss { get; set; } = string.Empty;
    }
}