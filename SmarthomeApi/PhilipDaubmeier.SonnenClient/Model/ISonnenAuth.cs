using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public interface ISonnenAuth
    {
        string AccessToken { get; }
        DateTime AccessTokenExpiry { get; }
        string RefreshToken { get; }

        string Username { get; }
        string UserPassword { get; }

        bool IsAccessTokenValid();

        Task UpdateTokenAsync(string accessToken, DateTime accessTokenExpiry, string refreshToken);
    }
}