using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.WeConnectClient.Network
{
    public interface IWeConnectAuth
    {
        string? AccessToken { get; }
        DateTime AccessTokenExpiry { get; }

        string Username { get; }
        string UserPassword { get; }

        bool IsAccessTokenValid();

        Task UpdateTokenAsync(string? accessToken, DateTime accessTokenExpiry, string? refreshToken);
    }
}