[![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.ViessmannClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.ViessmannClient/)
[![Build status](https://ci.appveyor.com/api/projects/status/mj67oe2c9wfkv2ld/branch/master?svg=true)](https://ci.appveyor.com/project/philipdaubmeier/graphiot/branch/master)

# ViessmannClient

This class library provides a way to call the Viessmann Server interfaces. It supports RESTful Viessmann Plattform services, the Estrella server as well as the legacy Vitotrol SOAP service. It encapsulates all authentication, retry and parsing logic and provides a strongly typed method interface.

## NuGet

```powershell
PM> Install-Package PhilipDaubmeier.ViessmannClient
```

## Usage

You have to implement the interfaces `IViessmannAuth` and `IViessmannConnectionProvider<T>` to provide the Viessmann webservice clients with all information necessary to authenticate and establish a connection.

The minimal viable example for playing around with the client would be as follows:

```csharp
public class ViessmannAuth : IViessmannAuth
{
    public string AccessToken { get; private set; }
    public DateTime AccessTokenExpiry { get; private set; }
    public string Username { get; }
    public string UserPassword { get; }
    public bool IsAccessTokenValid() => AccessTokenExpiry > DateTime.Now
                                        && !string.IsNullOrEmpty(AccessToken);

    public ViessmannAuth(string username, string password)
    {
        Username = username;
        UserPassword = password;
    }

    public Task UpdateTokenAsync(string token, DateTime expiry, string refresh)
    {
        AccessToken = token;
        AccessTokenExpiry = expiry;
        return Task.CompletedTask;
    }
}
```

And to create a connection provider you can use the default implementation `ViessmannConnectionProvider<T>`:

```csharp
var viessmannConnProvider = new ViessmannConnectionProvider<ViessmannPlatformClient>()
{
    AuthData = new ViessmannAuth("<username>", "<password>"),
    PlattformApiClientId = "<clientId>",
    PlattformApiClientSecret = "<clientSecret>"
};
```

> **Caution:** in a productive use you may want to implement your own `IViessmannConnectionProvider<T>` and load your app id, secret and user credentials from a suitable vault. You can have a look at the respective classes in [`GraphIoT.Viessmann`](../GraphIoT.Viessmann/Config) as an example for `IViessmannAuth` and `IViessmannConnectionProvider<T>` implemenations with secrets injection and storing/loading of access tokens.

If you have the connection providers in place, you can create and use the Viessmann client like this:

```csharp
var client = new ViessmannPlatformClient(viessmannPlatformConnProvider);

// Get the first installation, gateway and device id of the logged in user
var installationId = (await client.GetInstallations()).Data.First().Id ?? 0;
var gatewayId = (await client.GetGateways(installationId)).Data.First().Id;
var deviceId = (await client.GetDevices(installationId, gatewayId)).Data.First().LongId;

// Get sensor values
var features = await client.GetFeatures(installationId, gatewayId, deviceId);
var outsideTemp = features.GetFeature(FeatureName.Name.HeatingSensorsTemperatureOutside)?.ValueAsDouble;
var boilerTemp = features.GetFeature(FeatureName.Name.HeatingBoilerTemperature)?.ValueAsDouble;
Console.WriteLine($"Outside temp: {outsideTemp} °C, boiler temp: {boilerTemp} °C");
```

## Platform Support

ViessmannClient is compiled for .NET Core 3.0.

## License

The MIT License (MIT)

Copyright (c) 2020 Philip Daubmeier

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
