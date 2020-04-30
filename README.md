![GraphIoT logo](doc/img/graphiot_logo.svg)

[![Build status](https://ci.appveyor.com/api/projects/status/mj67oe2c9wfkv2ld/branch/master?svg=true)](https://ci.appveyor.com/project/philipdaubmeier/graphiot/branch/master)

![Azure pipeline](https://dev.azure.com/philipdaubmeier/GraphIoT/_apis/build/status/philipdaubmeier-graphiot%20-%20CI)

# GraphIoT

GraphIoT is a .NET Core project for polling and storing historical IoT and smart home sensor data and providing it for visualization as time series graphs.

The main server application consists of these two major parts for this purpose:

1. **Data Gathering**
    * A set of timed hosted services that regularly poll data from other servers, e.g. home servers of smart home systems or cloud apis of smart home device manufacturers. These values are stored in a configurable database. Currently it supports polling from:
        * Digitalstrom DSS (home server)
        * Netatmo (Cloud API)
        * Sonnen (Cloud API)
        * Viessmann (Cloud API)
        * WeConnect (Cloud API)
2. **Data Visualization**
    * A set of RESTful APIs to retrieve the time series data
    * Grafana visualization via:
        * a grafana hosted service that starts and monitors the grafana server process
        * a reverse proxy middleware that redirects to the grafana server
        * a REST+json api that loads, aggregates and preprocesses the stored data and outputs it in a grafana JSON datasource compatible format

## Code Structure

GraphIoT consists of several subprojects, some of which are independent of GraphIoT dependencies and can be used within other applications as nuget packages.

It is structured as follows:

* **Client Libaries** for communicating with a specific IoT device type:
  * [![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.DigitalstromClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.DigitalstromClient/) [DigitalstromClient](src/DigitalstromClient)
  * [![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.DigitalstromTwin.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.DigitalstromTwin/) [DigitalstromTwin](src/DigitalstromTwin)
  * [![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.NetatmoClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.NetatmoClient/) [NetatmoClient](src/NetatmoClient)
  * [![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.SonnenClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.SonnenClient/) [SonnenClient](src/SonnenClient)
  * [![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.ViessmannClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.ViessmannClient/) [ViessmannClient](src/ViessmannClient)
  * [![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.WeConnectClient.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.WeConnectClient/) [WeConnectClient](src/WeConnectClient)
* **Shared Libraries** for common functionality:
  * [![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.CompactTimeSeries.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.CompactTimeSeries/) [CompactTimeSeries](src/CompactTimeSeries)
  * [![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.TokenStore.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.TokenStore/) [TokenStore](src/TokenStore)
* **Core Library** for all GraphIoT Host libraries:
  * [GraphIoT.Core](src/GraphIoT.Core)
* **Host Libraries** for a specific IoT device type:
  * [GraphIoT.Digitalstrom](src/GraphIoT.Digitalstrom)
  * [GraphIoT.Netatmo](src/GraphIoT.Netatmo)
  * [GraphIoT.Sonnen](src/GraphIoT.Sonnen)
  * [GraphIoT.Viessmann](src/GraphIoT.Viessmann)
* **Visualization Library**:
  * [GraphIoT.Grafana](src/GraphIoT.Grafana)
* **Main Host Application**:
  * [GraphIoT.App](src/GraphIoT.App)

## Setup

Follow these steps to set up the development environment, publish the application to your own server and create Grafana dashboards with the actual graphs.

#### Setup application

* [Setup dev environment](doc/setup/setup_development.md)
* [Setup production environment](doc/setup/setup_production.md)
* [Backup database](doc/setup/backup_database.md)

#### Configure Grafana

* [Configure datasource](doc/grafana/configure_datasource.md)
* [Create dashboard and panels](doc/grafana/configure_dashboard.md)
* [Add variables and annotations](doc/grafana/configure_variables_annotations.md)

## Platform Support

GraphIoT is compiled for .NET Core 3.1.

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
