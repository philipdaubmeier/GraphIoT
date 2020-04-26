using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient.Model.Auth
{
    public interface IDigitalstromAuth : IDeepCloneable<IDigitalstromAuth>
    {
        string AppId { get; }
        string? ApplicationToken { get; }
        DateTime SessionExpiration { get; }
        string? SessionToken { get; }
        string Username { get; }
        string UserPassword { get; }

        bool MustFetchApplicationToken();
        bool MustFetchSessionToken();
        Task TouchSessionTokenAsync();
        Task UpdateTokenAsync(string? sessionToken, DateTime sessionExpiration, string applicationToken);
    }
}