using Newtonsoft.Json;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class ModuleBase
    {
        [JsonProperty("_id")]
        public ModuleId Id { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Firmware { get; set; }
        public ModuleData DashboardData { get; set; } = new ModuleData();

        private List<Measure> _dataType = new List<Measure>();
        public List<Measure> DataType
        {
            get
            {
                return _dataType;
            }
            set
            {
                // Special case for wind module: add all 4 wind measures that can be read
                if (value != null && value.Count == 1 && value[0] == MeasureType.WindStrength)
                    _dataType = new List<Measure>() { MeasureType.WindStrength, MeasureType.WindAngle, MeasureType.Guststrength, MeasureType.GustAngle };
                else if (value != null)
                    _dataType = value;
            }
        }
    }
}