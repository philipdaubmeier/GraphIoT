[![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.ViessmannClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.ViessmannClient/)

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

public class ViessmannConnProvider<T> : IViessmannConnectionProvider<T>
{
    public IViessmannAuth AuthData => new ViessmannAuth("<username>", "<password>");
    public HttpMessageHandler Handler => null;
    public string VitotrolDeviceId => "<deviceId>";
    public string VitotrolInstallationId => "<installationId>";
    public string PlattformInstallationId => "<installationId>";
    public string PlattformGatewayId => "<gatewayId>";
    public string PlattformApiClientId => "<clientId>";
    public string PlattformApiClientSecret => "<clientSecret>";
}
```

> **Caution:** in a productive use you may want to load your app id, secret and user credentials from a suitable vault. You can have a look at the respective classes in [`GraphIoT.Viessmann`](../GraphIoT.Viessmann/Config) as an example for `IViessmannAuth` and `IViessmannConnectionProvider<T>` implemenations with secrets injection and storing/loading of access tokens.

If you have the connection provider class in place, you can create and use the Viessmann clients like this:

```csharp
// Get the gateway and all controllers via Estrella server
var estrellaClient = new ViessmannEstrellaClient(new ViessmannConnProvider<ViessmannEstrellaClient>());
var gatewayIds = await estrellaClient.GetGateways();
foreach (var gatewayId in gatewayIds)
    foreach (var controller in await estrellaClient.GetControllers(gatewayId))
        Console.WriteLine($"Controller id: {controller.Item1} name: {controller.Item2}");

// Get raw datapoints from Vitotrol client, e.g. solar production (id: 7895)
var vitotrolClient = new ViessmannVitotrolClient(new ViessmannConnProvider<ViessmannVitotrolClient>());
var solarWhTotalTuple = (await vitotrolClient.GetData(new List<int>() { 7895 })).First();
var timestamp = solarWhTotalTuple.Item3.ToUniversalTime();
var solarWhTotal = int.Parse(solarWhTotalTuple.Item2);
Console.WriteLine($"Total solar production today id: {solarWhTotal} Wh, time: {timestamp}");

// Get heating sensor values via Viessmann Platform client
var platformClient = new ViessmannPlatformClient(new ViessmannConnProvider<ViessmannPlatformClient>());
var outsideTemp = (await platformClient.GetOutsideTemperature()).Item2;
var boilerTemp = await platformClient.GetBoilerTemperature();
Console.WriteLine($"Outside temp: {outsideTemp} °C, boiler temp: {boilerTemp} °C");
```

## Platform Support

CompactTimeSeries is compiled for .NET Core 2.2.

## License

The MIT License (MIT)

Copyright (c) 2019 Philip Daubmeier

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
