using System;

namespace DigitalstromClient.Model
{
    public interface IDigitalstromAuth
    {
        string AppId { get; }
        string ApplicationToken { get; set; }
        DateTime SessionExpiration { get; set; }
        string SessionToken { get; set; }
        string Username { get; }
        string UserPassword { get; }

        bool MustFetchApplicationToken();
        bool MustFetchSessionToken();
        void TouchSessionToken();
    }
}