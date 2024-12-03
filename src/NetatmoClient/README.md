[![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.NetatmoClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.NetatmoClient/)
[![Build status](https://ci.appveyor.com/api/projects/status/mj67oe2c9wfkv2ld/branch/master?svg=true)](https://ci.appveyor.com/project/philipdaubmeier/graphiot/branch/master)

# NetatmoClient

NetatmoClient is a .NET Core class library with a wrapper for the Netatmo Cloud RESTful JSON web service. It encapsulates all authentication, retry and parsing logic and provides a strongly typed method interface of the Netatmo API.

## NuGet

```powershell
PM> Install-Package PhilipDaubmeier.NetatmoClient
```

## Usage

You have to implement the interface `INetatmoConnectionProvider` to provide the `NetatmoWebClient` with all information necessary to authenticate at the API and establish a connection or use the existing `NetatmoConnectionProvider`.

The minimal viable example for playing around with the client would be as follows:

```csharp
var netatmoAuth = new NetatmoAuth();
var netatmoConnProvider = new NetatmoConnectionProvider(netatmoAuth)
{
    AppId = "<your_netatmo_connect_app_id>",
    AppSecret = "<your_netatmo_connect_app_secret>",
    Scope = "read_station read_presence access_presence",
    RedirectUri = "http://localhost:4000"
};

var netatmoClient = new NetatmoWebClient(netatmoConnProvider);
var uri = netatmoClient.GetLoginUri();

Console.WriteLine($"Login here: {uri.AbsoluteUri}");
```

For playing around you can now copy the uri that was written to the console into a browser and log in there. The browser will then redirect to a page that does not exist and will show a _"page not found"_ message. Just use the part after `?code=` in the browser address bar and input it into the program, which is shown in the following.

```csharp
Console.WriteLine("After logging in you should be redirected to a non-existent page.");
Console.WriteLine("Enter the code you see in the browsers address bar behind '?code=':");
var code = Console.ReadLine();
await netatmoClient.TryCompleteLogin(code);
```

After this step, the `auth` object will contain a valid access token and also a refresh token. The refresh token can be permanently persisted and loaded after each startup and will automatically be used for refreshing the access token if expired.

> **Note:** in a productive use you will want to launch the login uri in an embedded browser view or redirect to this uri in case of a web application. After sucessful login either capture the resulting uri from the embedded browser or use a productive callback API on your server side.
>
> **Note:** also, you may want to implement your own `INetatmoConnectionProvider` and load your client id and redirect uri from a configuration file and store and load refresh tokens across program restarts. You can have a look at the respective classes in [`GraphIoT.Netatmo`](../GraphIoT.Netatmo/Config) as an example for `INetatmoAuth` and `INetatmoConnectionProvider` implemenations with storing/loading of configuration, access tokens and refresh tokens.

With being logged in sucessfully and having a valid refresh token in the `auth` object, you can now go ahead and use the library to actually query station data and measurements:

```csharp
// Find the id of the first base station of the logged in user
var weatherStation = await netatmoClient.GetWeatherStationData();
var baseStationId = weatherStation.Devices.First().Id;

// Get temperature values of the past 24 hours of the base station
var start = DateTime.Now.AddDays(-1);
var end = DateTime.Now;
var measureTypes = new Measure[] { MeasureType.Temperature };
var measures = await netatmoClient.GetMeasure(baseStationId, baseStationId,
                     measureTypes, MeasureScale.Scale1Hour, start, end);

// Print out all values
foreach (var measure in measures.First().Value)
    Console.WriteLine($"timestamp: {measure.Key} temperature: {measure.Value} °C");
```

## Platform Support

NetatmoClient is targeted for .NET 9.0 or higher.

## License

The MIT License (MIT)

Copyright (c) 2019-2024 Philip Daubmeier

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
