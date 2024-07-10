[![NuGet](http://img.shields.io/nuget/v/PhilipDaubmeier.CompactTimeSeries.svg?style=flat-square)](https://www.nuget.org/packages/PhilipDaubmeier.CompactTimeSeries/)
[![Build status](https://ci.appveyor.com/api/projects/status/mj67oe2c9wfkv2ld/branch/master?svg=true)](https://ci.appveyor.com/project/philipdaubmeier/graphiot/branch/master)

# CompactTimeSeries

CompactTimeSeries is an in-memory time series library for space-efficient storing, processing, resampling and compressing of time series data.

## NuGet

```powershell
PM> Install-Package PhilipDaubmeier.CompactTimeSeries
```

## When should I use it?

If you need to deal with mostly equidistant time series with a large number of values, e.g. sensor readings that are equally spaced in time, and need to process them in memory. The main classes of this library, which implement `ITimeSeries<T>` allow for convenient random access read/write to a time series via time-based indexers.

If you need fast persisting/loading of such time series data, this library will not help you - just use a [time series database](https://en.wikipedia.org/wiki/Time_series_database#List_of_time_series_databases) like Influx, Graphite, CrateDB etc.

## Usage

```csharp
// Create a time series for today with 1 second spacing, which will result in 86400 values
var span = new TimeSeriesSpan(DateTime.Now.Date, DateTime.Now.Date.AddDays(1), TimeSeriesSpan.Spacing.Spacing1Sec);
var timeseries = new TimeSeriesStream<int>(span);

// Write to the time series via time-based indexer: the time bucket for the current second will be written
timeseries[DateTime.Now] = 42;

// Enumerate the time series values and timestamps like an IEnumerable<KeyValuePair<DateTime, T?>>
foreach (var timestampedValue in timeseries)
    DoSomething(timestampedValue);
```

For more usage examples you can also have a look at the unit tests.

## Platform Support

CompactTimeSeries is targeted for .NET 8.0 or higher.

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
