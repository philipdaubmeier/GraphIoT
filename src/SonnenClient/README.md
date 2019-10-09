[![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.SonnenClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.SonnenClient/)

# SonnenClient

SonnenClient is a .NET Core class library with a wrapper for the [Sonnen Portal](https://account.sonnen.de/) RESTful JSON interface. It encapsulates all authentication, retry and parsing logic and provides a strongly typed method interface.

## NuGet

```powershell
PM> Install-Package PhilipDaubmeier.SonnenClient
```

## Usage

You have to implement the interface `ISonnenConnectionProvider` to provide the `SonnenPortalClient` with all information necessary to authenticate at the API and establish a connection.

The minimal viable example for playing around with the client would be as follows:

```csharp
public class SonnenConnProvider : ISonnenConnectionProvider
{
    public ISonnenAuth AuthData => new SonnenAuth("<username>", "<password>");
    public HttpMessageHandler Handler => null;

    public string ClientId => "<client_id>";
}
```

> **Caution:** in a productive use you may want to load your client id from a suitable vault and the user credentials should be entered by the user in some way and immediatelly discarded again. The `ISonnenAuth` object will contain a refresh token that can be used to re-authenticate at any time, which can be persisted by implementing a custom `ISonnenAuth` class. You can have a look at the respective classes in [`GraphIoT.Sonnen`](../GraphIoT.Sonnen/Config) as an example.

If you have the connection provider class in place, you can create a `SonnenPortalClient` and query for battery info and measurements:

```csharp
var sonnenClient = new SonnenPortalClient(new SonnenConnProvider());
            
// get the default Sonnen battery site
var siteId = (await sonnenClient.GetUserSites()).DefaultSiteId;

// get the first battery of this site
var battery = (await sonnenClient.GetBatterySystems(siteId)).First();
Console.WriteLine($"Charge cycles: {battery.BatteryChargeCycles}");

// get statistics
var stats = await sonnenClient.GetStatistics(siteId, DateTime.Now.AddDays(-1), DateTime.Now);
Console.WriteLine($"Produced energy (last 24h): {stats.ProducedEnergy}");

// get high resolution measurements and print the last 10 values of the time series
var graph = await sonnenClient.GetEnergyMeasurements(siteId, DateTime.Now.AddDays(-1), DateTime.Now);
var productionWattGraph = graph.ProductionPower;
Console.WriteLine($"Production power: {string.Join(',', productionWattGraph.TakeLast(10))}");
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
