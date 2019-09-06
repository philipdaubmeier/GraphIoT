using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using PhilipDaubmeier.NetatmoClient.Model.WeatherStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.NetatmoHost.Polling
{
    public class NetatmoWeatherPollingService : INetatmoPollingService
    {
        private readonly ILogger _logger;
        private readonly NetatmoWebClient _netatmoClient;

        private Dictionary<Tuple<ModuleId, ModuleId>, List<Measure>> _modules = null;

        public NetatmoWeatherPollingService(ILogger<NetatmoWeatherPollingService> logger, NetatmoWebClient netatmoClient)
        {
            _logger = logger;
            _netatmoClient = netatmoClient;
        }

        public async Task PollValues()
        {
            _logger.LogInformation($"{DateTime.Now} Netatmo Background Service is polling new weather station values...");

            try
            {
                await PollSensorValues(DateTime.Now.AddHours(-1), DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in Netatmo weather background worker: {ex.Message}");
            }
        }

        public async Task PollSensorValues(DateTime start, DateTime end)
        {
            if (_modules == null)
            {
                var stationdata = await _netatmoClient.GetWeatherStationData();

                // TODO: remove hardcoded condition that the station contains "phils"
                _modules = stationdata.Devices
                    .Where(s => s.StationName.Contains("phils", StringComparison.InvariantCultureIgnoreCase))
                    .SelectMany(station => new[] { (ModuleBase)station }
                        .Union(station.Modules.Cast<ModuleBase>())
                        .ToDictionary(m => new Tuple<ModuleId, ModuleId>(station.Id, m.Id), m => m.DataType)
                    )
                    .ToDictionary(module => module.Key, module => module.Value);
            }

            var days = new TimeSeriesSpan(start, end, 1).IncludedDates().ToList();
            var timeseries = new Dictionary<ModuleId, Dictionary<Measure, List<TimeSeries<double>>>>();
            foreach (var module in _modules)
            {
                var measures = await _netatmoClient.GetMeasure(module.Key.Item1, module.Key.Item2, module.Value, MeasureScale.ScaleMax, start, end, null, true);

                var series = module.Value.Where(m => m != null).ToDictionary(m => m, m => Enumerable.Range(0, days.Count).Select(k =>
                        new TimeSeries<double>(new TimeSeriesSpan(days[k], days[k].AddDays(1), TimeSeriesSpan.Spacing.Spacing5Min))).ToList());
                foreach (var measure in measures)
                    foreach (var originSeries in measure.Value)
                        for (int k = 0; k < series[measure.Key].Count; k++)
                            series[measure.Key][k][originSeries.Key] = originSeries.Value;
                timeseries.Add(module.Key.Item2, series);
            }

            {
                // This block is for finding out which decimal places should be taken for each measure
                // as we have a maximum range of -32768 to 32767 when storing, and 10 times less for each
                // decimal place we configure
                var groupedByMeasure = timeseries.SelectMany(m => m.Value.Select(x => new Tuple<Tuple<ModuleId, Measure>, List<TimeSeries<double>>>(new Tuple<ModuleId, Measure>(m.Key, x.Key), x.Value)))
                                                 .SelectMany(x => x.Item2.Select(m => new Tuple<Measure, TimeSeries<double>>(x.Item1.Item2, m))).GroupBy(x => x.Item1, x => x.Item2);
                Dictionary<Measure, double> CondenseToSingleValuePerMeasure(Func<IEnumerable<double>, double> aggregatorFunc)
                {
                    return groupedByMeasure.ToDictionary(x => x.Key, x => aggregatorFunc(x.SelectMany(t => t.Where(v => v.Value.HasValue).Select(v => v.Value.Value))));
                }
                var minValues = CondenseToSingleValuePerMeasure(x => x.Min());
                var maxValues = CondenseToSingleValuePerMeasure(x => x.Max());
                var wholeDict = groupedByMeasure.ToDictionary(x => x.Key, x => x.SelectMany(t => t.Where(v => v.Value.HasValue).Select(v => v.Value.Value)));

                // results for the last 10 months, and added knowhow about plausible min/max values:
                //  measure      minValues maxValues decimalPlaces -> allows for range
                //  temperature: -50       29.6      1                -3276.8 to 3276.7
                //  co2:         350       5000      0                -32768 to 32767
                //  humidity:    46        100       1                -3276.8 to 3276.7
                //  noise:       33        75        1                -3276.8 to 3276.7
                //  pressure:    1009.4    1024.1    1                -3276.8 to 3276.7
                //  rain:        0         2.828     3                -32.768 to 32.767
                //  windstrength 0         6         1                -3276.8 to 3276.7
                //  windangle    -1        360       1                -3276.8 to 3276.7
                //  guststrength 1         12        1                -3276.8 to 3276.7
                //  gustangle    -1        360       1                -3276.8 to 3276.7

                // all modules of both locations with all measures combined: 33
            }

            // TODO: store loaded timeseries
            var do_something_with = timeseries;
        }
    }
}