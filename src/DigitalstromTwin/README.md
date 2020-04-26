[![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.DigitalstromTwin.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.DigitalstromTwin/)
[![Build status](https://ci.appveyor.com/api/projects/status/mj67oe2c9wfkv2ld/branch/master?svg=true)](https://ci.appveyor.com/project/philipdaubmeier/graphiot/branch/master)

# DigitalstromTwin

DigitalstromTwin is a class library providing a DigitalstromDssTwin object as the main class, that automatically synchronizes scene states of all rooms both ways, i.e. if a state is changed programatically it sends a command to the DSS, if a scene is changed within the apartement the event is synched into the twin model.

## NuGet

```powershell
PM> Install-Package PhilipDaubmeier.DigitalstromTwin
```

## Usage

You have to implement the interface `IDigitalstromConnectionProvider` to provide the `DigitalstromDssTwin` with all information necessary to authenticate at the API and establish a connection, or use the default implementation `DigitalstromConnectionProvider`. See [DigitalstromClient usage documentation](../DigitalstromClient/README.md) for an example of this.

Having a connection provider, you can create a `DigitalstromDssTwin` and use it like:

```csharp
// Create the twin
var dssTwin = new DigitalstromDssTwin(connProvider);

// Get notified by events if something changed on the DSS
dssTwin.CollectionChanged += (s, e) => Console.WriteLine("New room detected");
dssTwin.SceneChanged += (s, e) => Console.WriteLine($"Scene in zone {e.Zone}, " + 
                                    $"group {e.Group} was changed to {e.Scene}");

Zone zoneKitchen = 5;

// Read the last called scene by simply using the indexer operator
var kitchenLightScene = dssTwin[zoneKitchen, Color.Yellow];

// Call a scene on the DSS by just setting the desired value on the twin
dssTwin[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset2;
```

For more usage examples you can also have a look at the [unit tests](../../test/DigitalstromTwin.Tests).

## Platform Support

DigitalstromTwin is targeted for .NET Standard 2.1 or higher.

## License

The MIT License (MIT)

Copyright (c) 2019-2020 Philip Daubmeier

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
