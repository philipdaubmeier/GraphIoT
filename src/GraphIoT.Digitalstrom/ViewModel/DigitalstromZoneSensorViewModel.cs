using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Structure;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.ViewModel
{
    public class DigitalstromZoneSensorViewModel : GraphCollectionViewModelKeyedItemBase<DigitalstromZoneSensorData, Zone>
    {
        private readonly IDigitalstromStructureService _dsStructure;

        public DigitalstromZoneSensorViewModel(IDigitalstromDbContext databaseContext, IDigitalstromStructureService dsStructure)
            : base(new Dictionary<Resolution, IQueryable<DigitalstromZoneSensorData>>() {
                       { Resolution.LowRes, databaseContext.DsSensorLowresDataSet },
                       { Resolution.MidRes, databaseContext.DsSensorDataSet }
                   },
                   Enumerable.Range(0, 2).ToDictionary(x => x.ToString(), x => x),
                   dsStructure.Zones.Where(x => dsStructure.HasZoneSensor(x, SensorType.TemperatureIndoors)
                                             || dsStructure.HasZoneSensor(x, SensorType.HumidityIndoors)).OrderBy(x => x).ToList(),
                   x => x.ZoneId,
                   key => { int keyint = key; return x => x.ZoneId == keyint; })
        {
            _dsStructure = dsStructure;
        }

        public override string Key => "sensors";

        public override GraphViewModel Graph(int index)
        {
            int column = index % _columns.Count;
            return column switch
            {
                0 => DeferredLoadGraph<TimeSeries<double>, double>(index, k => $"Temperatur {_dsStructure?.GetZoneName(k) ?? k.ToString()}", k => $"temperatur_zone_{(int)k}", "#.# °C"),
                1 => DeferredLoadGraph<TimeSeries<double>, double>(index, k => $"Luftfeuchtigkeit {_dsStructure?.GetZoneName(k) ?? k.ToString()}", k => $"luftfeuchtigkeit_zone_{(int)k}", "#.0 '%'"),
                _ => new GraphViewModel(),
            };
        }
    }
}