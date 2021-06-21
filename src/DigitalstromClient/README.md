[![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.DigitalstromClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.DigitalstromClient/)
[![Build status](https://ci.appveyor.com/api/projects/status/mj67oe2c9wfkv2ld/branch/master?svg=true)](https://ci.appveyor.com/project/philipdaubmeier/graphiot/branch/master)

# DigitalstromClient

DigitalstromClient is a .NET Core class library with a wrapper for the Digitalstrom Server (DSS) RESTful JSON interface. It encapsulates all authentication, retry and parsing logic and provides a strongly typed method interface of the DSS.

## NuGet

```powershell
PM> Install-Package PhilipDaubmeier.DigitalstromClient
```

## Usage

You have to implement the interface `IDigitalstromConnectionProvider` to provide the `DigitalstromDssClient` with all information necessary to authenticate at the API and establish a connection, or use the default implementation `DigitalstromConnectionProvider`.

The minimal viable example for playing around (do not hardcode credentials in real use!) with the client would be as follows:

```csharp
var uri = new Uri("https://dss.local:8080");

// The issued token will be visible in the DSS configurator with the name "SampleAppId"
var credentials = new EphemeralDigitalstromAuth("SampleAppId", "<username>", "<password>");

// Create a connection provider with dss uri and credentials, accept any certificate
var connProvider = new DigitalstromConnectionProvider(uri, () => credentials, cert => true);
```

> **Caution:** In productive use you may want to let the user enter his credentials in some way and immediatelly discard them again. The `IDigitalstromAuth` object will contain an application token that can be used to refresh the session token at any time, which can be persisted by implementing a custom `IDigitalstromAuth` class. You can have a look at the respective classes in [`GraphIoT.Digitalstrom`](../GraphIoT.Digitalstrom/Config) as an example how this could look like.

> **Caution:** in productive use you may want to let the user check the certificate fingerprint at first use and store this for later certificate pinning, only accepting this exact certificate to detect MITM attempts. The certificate validation callback of `DigitalstromConnectionProvider` can be used for this purpose.

If you have the connection provider, you can create a `DigitalstromDssClient` and query the DSS for data:

```csharp
// Create the client
var dssClient = new DigitalstromDssClient(connProvider);

// Load all zones and their last called scenes
var lastCalledScenes = await dssClient.GetZonesAndLastCalledScenes();
foreach (var zone in lastCalledScenes.Zones)
{
    var yellowScene = zone.Groups.First(g => g.Group == Color.Yellow).LastCalledScene;
    Console.WriteLine($"zone id {zone.ZoneID} has yellow group scene: {yellowScene.ToString("d")}");
}
```

For more usage examples you can also have a look at the [unit tests](../../test/DigitalstromClient.Tests).

## Platform Support

DigitalstromClient is targeted for .NET Standard 2.1 or higher.

## License

The MIT License (MIT)

Copyright (c) 2019-2021 Philip Daubmeier

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
