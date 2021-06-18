using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannClient.Network
{
    public interface IViessmannAuth
    {
        string? AccessToken { get; }
        DateTime AccessTokenExpiry { get; }
        string? RefreshToken { get; }

        bool IsAccessTokenValid();

        Task UpdateTokenAsync(string? accessToken, DateTime accessTokenExpiry, string? refreshToken);
    }
}