[![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.ViessmannClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.ViessmannClient/)
[![Build status](https://ci.appveyor.com/api/projects/status/mj67oe2c9wfkv2ld/branch/master?svg=true)](https://ci.appveyor.com/project/philipdaubmeier/graphiot/branch/master)

# ViessmannClient

This class library provides a way to call the Viessmann Platform API. It encapsulates all authentication, retry and parsing logic and provides a strongly typed method interface for reading all details of an installation as well as all setting and sensor values of devices.

## NuGet

```powershell
PM> Install-Package PhilipDaubmeier.ViessmannClient
```

## Usage

You have to implement the interfaces `IViessmannAuth` and `IViessmannConnectionProvider<T>` to provide the Viessmann webservice clients with all information necessary to authenticate and establish a connection.

The minimal viable example for playing around (do not hardcode credentials in real use!) with the client would be to create a connection provider as shown here:

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
var client = new ViessmannPlatformClient(viessmannConnProvider);

// Get the first installation, gateway and device id of the logged in user
var installationId = (await client.GetInstallations()).First().LongId;
var gatewayId = (await client.GetGateways(installationId)).First().LongId;
var deviceId = (await client.GetDevices(installationId, gatewayId)).First().LongId;

// Get sensor values
var features = await client.GetDeviceFeatures(installationId, gatewayId, deviceId);
var outsideTemp = features.GetHeatingSensorsTemperatureOutside();
var boilerTemp = features.GetHeatingBoilerTemperature();
Console.WriteLine($"Outside temp: {outsideTemp} °C, boiler temp: {boilerTemp} °C");

// Query for properties of individual circuits
foreach (var circuit in features.GetHeatingCircuits())
    Console.WriteLine($"Name of {circuit}: {features.GetHeatingCircuitsCircuitName(circuit)}");
```

For more usage examples you can also have a look at the [unit tests](../../test/ViessmannClient.Tests).

A full list of status and sensor values of devices can be found in the [`DeviceFeatureList`](Model/Devices/DeviceFeatureList.cs) class.

## Platform Support

ViessmannClient is compiled for .NET Core 3.1.

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
