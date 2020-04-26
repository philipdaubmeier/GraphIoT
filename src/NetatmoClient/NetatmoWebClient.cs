using PhilipDaubmeier.NetatmoClient.Model.Core;
using PhilipDaubmeier.NetatmoClient.Model.HomeData;
using PhilipDaubmeier.NetatmoClient.Model.WeatherStation;
using PhilipDaubmeier.NetatmoClient.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.NetatmoClient
{
    public class NetatmoWebClient : NetatmoAuthBase
    {
        /// <summary>
        /// Connects to the netatmo REST webservice at the given uri with the given
        /// connection parameters.
        /// </summary>
        public NetatmoWebClient(INetatmoConnectionProvider connectionProvider)
            : base(connectionProvider)
        { }

        /// <summary>
        /// Returns data from a user's Weather Stations (measures and device specific data).
        /// </summary>
        /// <param name="deviceId">Weather station mac address. If null, all devices (or all favorites) are returned.</param>
        /// <param name="getFavorites">To retrieve user's favorite weather stations. Default is false.</param>
        public async Task<WeatherStationData> GetWeatherStationData(ModuleId? deviceId = default, bool? getFavorites = default)
        {
            var uri = new Uri($"{_baseUri}/api/getstationsdata");
            return await CallNetatmoApi<WeatherStationResponse, WeatherStationData>(uri, new[]
            {
                ("device_id", deviceId?.ToString()),
                ("get_favorites", getFavorites?.ToString())
            });
        }

        /// <summary>
        /// Returns information about users homes and cameras. This method is available
        /// for Welcome, Presence and the Smart Smoke Alarm.
        /// </summary>
        /// <param name="homeId">Specify if you're looking for the events of a specific Home. Example: "56c881f049c75fe562732029"</param>
        /// <param name="size">Number of events to retrieve. Default is 30.</param>
        public async Task<HomeData> GetHomeData(string? homeId = default, int? size = default)
        {
            var uri = new Uri($"{_baseUri}/api/gethomedata");
            return await CallNetatmoApi<HomeDataResponse, HomeData>(uri, new[]
            {
                ("home_id", homeId),
                ("size", !size.HasValue ? null : Math.Max(0, Math.Min(1024, size.Value)).ToString())
            });
        }

        /// <summary>
        /// Retrieve data from a device or module (Weather station and Thermostat only).
        /// </summary>
        /// <param name="deviceId">Mac address of the device (can be found via GetWeatherStationData)</param>
        /// <param name="moduleId">Mac address of the module you're interested in. If not specified, returns data of the device. If specified, returns data from the specified module.</param>
        /// <param name="types">Measures you are interested in. Data you can request depends on the scale.</param>
        /// <param name="scale">Timelapse between two measurements</param>
        /// <param name="dateBegin">Timestamp of the first measure to retrieve. Default is null.</param>
        /// <param name="dateEnd">Timestamp of the last measure to retrieve (default and max are 1024). Default is null.</param>
        /// <param name="limit">Maximum number of measurements (default and max are 1024)</param>
        /// <param name="realTime">If scale different than max, timestamps are by default offset + scale/2. To get exact timestamps, use true. Default is false.</param>
        public async Task<TimestampedMeasureCollection> GetMeasure(ModuleId deviceId, ModuleId moduleId, IEnumerable<Measure> types, Scale? scale = default, DateTime? dateBegin = default, DateTime? dateEnd = default, int? limit = default, bool? realTime = default)
        {
            var uri = new Uri($"{_baseUri}/api/getmeasure");
            return new TimestampedMeasureCollection(await CallNetatmoApi<MeasureResponse, List<MeasureClump>>(uri, new[]
            {
                ("device_id", (string)deviceId),
                ("module_id", (string)moduleId),
                ("scale", scale?.ToString() ?? new Scale(MeasureScale.ScaleMax).ToString()),
                ("type", string.Join(',', types.Select(t => t.ToString()))),
                ("date_begin", !dateBegin.HasValue ? null : new DateTimeOffset(dateBegin.Value.ToUniversalTime()).ToUnixTimeSeconds().ToString()),
                ("date_end", !dateEnd.HasValue ? null : new DateTimeOffset(dateEnd.Value.ToUniversalTime()).ToUnixTimeSeconds().ToString()),
                ("limit", !limit.HasValue ? null : Math.Max(0, Math.Min(1024, limit.Value)).ToString()),
                ("real_time", realTime?.ToString())
            }), types);
        }
    }
}