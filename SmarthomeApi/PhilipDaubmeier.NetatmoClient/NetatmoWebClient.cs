using NodaTime;
using PhilipDaubmeier.NetatmoClient.Model.HomeData;
using PhilipDaubmeier.NetatmoClient.Model.WeatherStation.WeatherStation;
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

        public async Task<WeatherStationData> GetWeatherStationData()
        {
            var uri = new Uri($"{_baseUri}/api/getstationsdata");
            return await CallNetatmoApi<WeatherStationResponse, WeatherStationData>(uri);
        }

        public async Task<HomeData> GetHomeData()
        {
            var uri = new Uri($"{_baseUri}/api/gethomedata");
            return await CallNetatmoApi<HomeDataResponse, HomeData>(uri);
        }

        public enum MeasureScale
        {
            ScaleMax,
            Scale30Min,
            Scale1Hour,
            Scale3Hours,
            Scale1Day,
            Scale1Week,
            Scale1Month
        }

        private string MeasureScaleToString(MeasureScale scale)
        {
            switch (scale)
            {
                case MeasureScale.Scale30Min: return "30min";
                case MeasureScale.Scale1Hour: return "1hour";
                case MeasureScale.Scale3Hours: return "3hours";
                case MeasureScale.Scale1Day: return "1day";
                case MeasureScale.Scale1Week: return "1week";
                case MeasureScale.Scale1Month: return "1month";
                case MeasureScale.ScaleMax: goto default;
                default: return "max";
            }
        }

        public enum MeasureType
        {
            Temperature,
            CO2,
            Humidity,
            Pressure,
            Noise,
            Rain,
            WindStrength,
            WindAngle,
            Guststrength,
            GustAngle,
            MinTemp,
            MaxTemp,
            MinHum,
            MaxHum,
            MinPressure,
            MaxPressure,
            MinNoise,
            MaxNoise,
            SumRain,
            MaxGust,
            DateMaxHum,
            DateMinPressure,
            DateMaxPressure,
            DateMinNoise,
            DateMaxNoise,
            DateMinCo2,
            DateMaxCo2,
            DateMaxGust
        }

        private string MeasureTypeToString(MeasureType type)
        {
            switch (type)
            {
                case MeasureType.Temperature: return "temperature";
                case MeasureType.CO2: return "co2";
                case MeasureType.Humidity: return "humidity";
                case MeasureType.Pressure: return "pressure";
                case MeasureType.Noise: return "noise";
                case MeasureType.Rain: return "rain";
                case MeasureType.WindStrength: return "windstrength";
                case MeasureType.WindAngle: return "windangle";
                case MeasureType.Guststrength: return "guststrength";
                case MeasureType.GustAngle: return "gustangle";
                case MeasureType.MinTemp: return "min_temp";
                case MeasureType.MaxTemp: return "max_temp";
                case MeasureType.MinHum: return "min_hum";
                case MeasureType.MaxHum: return "max_hum";
                case MeasureType.MinPressure: return "min_pressure";
                case MeasureType.MaxPressure: return "max_pressure";
                case MeasureType.MinNoise: return "min_noise";
                case MeasureType.MaxNoise: return "max_noise";
                case MeasureType.SumRain: return "sum_rain";
                case MeasureType.MaxGust: return "max_gust";
                case MeasureType.DateMaxHum: return "date_max_hum";
                case MeasureType.DateMinPressure: return "date_min_pressure";
                case MeasureType.DateMaxPressure: return "date_max_pressure";
                case MeasureType.DateMinNoise: return "date_min_noise";
                case MeasureType.DateMaxNoise: return "date_max_noise";
                case MeasureType.DateMinCo2: return "date_min_co2";
                case MeasureType.DateMaxCo2: return "date_max_co2";
                case MeasureType.DateMaxGust: return "date_max_gust";
                default: goto case MeasureType.Temperature;
            }
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
        /// <returns></returns>
        public async Task<List<TimestampedMeasureCollection>> GetMeasure(string deviceId, string moduleId, IEnumerable<MeasureType> types, MeasureScale scale = MeasureScale.ScaleMax, DateTime? dateBegin = default, DateTime? dateEnd = default, int? limit = default, bool? realTime = default)
        {
            var uri = new Uri($"{_baseUri}/api/getmeasure");

            return await CallNetatmoApi<MeasureResponse, List<TimestampedMeasureCollection>>(uri, new Dictionary<string, string>()
            {
                { "device_id", deviceId },
                { "module_id", moduleId },
                { "scale", MeasureScaleToString(scale) },
                { "type", types == null ? null : string.Join(',', types.Select(t => MeasureTypeToString(t))) },
                { "date_begin", !dateBegin.HasValue ? null : Instant.FromDateTimeUtc(dateBegin.Value.ToUniversalTime()).ToUnixTimeSeconds().ToString() },
                { "date_end", !dateEnd.HasValue ? null : Instant.FromDateTimeUtc(dateEnd.Value.ToUniversalTime()).ToUnixTimeSeconds().ToString() },
                { "limit", !limit.HasValue ? null : Math.Max(0, Math.Min(1024, limit.Value)).ToString() },
                { "real_time", realTime?.ToString() }
            });
        }
    }
}