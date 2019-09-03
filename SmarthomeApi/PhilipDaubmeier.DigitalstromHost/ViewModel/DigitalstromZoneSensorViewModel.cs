using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.DigitalstromHost.Structure;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromHost.ViewModel
{
    public class DigitalstromZoneSensorViewModel : GraphCollectionViewModelKeyedItemBase<DigitalstromZoneSensorData, Zone>
    {
        private readonly IDigitalstromStructureService _dsStructure;

        public DigitalstromZoneSensorViewModel(IDigitalstromDbContext databaseContext, IDigitalstromStructureService dsStructure)
            : base(new Dictionary<Resolution, DbSet<DigitalstromZoneSensorData>>() {
                       { Resolution.MidRes, databaseContext?.DsSensorDataSet }
                   },
                   Enumerable.Range(0, 2).ToDictionary(x => x.ToString(), x => x),
                   dsStructure.Zones.Where(x => dsStructure.HasZoneSensor(x, SensorType.TemperatureIndoors)
                                             || dsStructure.HasZoneSensor(x, SensorType.HumidityIndoors)).OrderBy(x => x).ToList(),
                   x => x.ZoneId)
        {
            _dsStructure = dsStructure;
        }

        public override string Key => "sensors";

        public override GraphViewModel Graph(int index)
        {
            int column = index % _columns.Count;
            switch (column)
            {
                case 0: return DeferredLoadGraph<TimeSeries<double>, double>(index, k => $"Temperatur {_dsStructure?.GetZoneName(k) ?? k.ToString()}", k => $"temperatur_zone_{(int)k}", "#.# °C");
                case 1: return DeferredLoadGraph<TimeSeries<double>, double>(index, k => $"Luftfeuchtigkeit {_dsStructure?.GetZoneName(k) ?? k.ToString()}", k => $"luftfeuchtigkeit_zone_{(int)k}", "#.0 '%'");
                default: return new GraphViewModel();
            }
        }
    }
}