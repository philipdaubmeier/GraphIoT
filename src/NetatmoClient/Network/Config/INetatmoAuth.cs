using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.NetatmoClient.Network
{
    public interface INetatmoAuth
    {
        string AccessToken { get; }
        DateTime AccessTokenExpiry { get; }
        string RefreshToken { get; }

        string Username { get; }
        string UserPassword { get; }

        bool IsAccessTokenValid();

        bool MustAuthenticate();

        bool MustRefreshToken();

        Task UpdateTokenAsync(string accessToken, DateTime accessTokenExpiry, string refreshToken);
    }
}