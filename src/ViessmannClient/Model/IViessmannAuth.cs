using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannClient.Model
{
    public interface IViessmannAuth
    {
        string? AccessToken { get; }
        DateTime AccessTokenExpiry { get; }

        string Username { get; }
        string UserPassword { get; }

        bool IsAccessTokenValid();

        Task UpdateTokenAsync(string? accessToken, DateTime accessTokenExpiry, string? refreshToken);
    }
}