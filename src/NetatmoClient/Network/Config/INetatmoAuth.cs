using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.NetatmoClient.Network
{
    public interface INetatmoAuth
    {
        string? AccessToken { get; }
        DateTime AccessTokenExpiry { get; }
        string? RefreshToken { get; }

        bool IsAccessTokenValid();

        Task UpdateTokenAsync(string? accessToken, DateTime accessTokenExpiry, string? refreshToken);
    }
}