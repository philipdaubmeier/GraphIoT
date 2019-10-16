using Newtonsoft.Json;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.ViewModel
{
    public class DigitalstromSceneEventViewModel : EventCollectionViewModel
    {
        private readonly IDigitalstromDbContext _db;
        private readonly IDigitalstromStructureService _dsStructure;

        public DigitalstromSceneEventViewModel(IDigitalstromDbContext databaseContext, IDigitalstromStructureService dsStructure) : base()
        {
            _db = databaseContext;
            _dsStructure = dsStructure;
        }

        public override string Key => "sceneevents";

        public override IEnumerable<EventViewModel> Events => EnumerateEvents();

        private IEnumerable<EventViewModel> EnumerateEvents()
        {
            var eventData = _db.DsSceneEventDataSet.Where(x => x.Key >= Span.Begin.Date && x.Key <= Span.End.Date).ToList();
            var events = eventData.SelectMany(x => x.EventStream).Where(x => x.TimestampUtc >= Span.Begin.ToUniversalTime() && x.TimestampUtc <= Span.End.ToUniversalTime()).ToList();
            List<DssEvent> filtered;

            try
            {
                var definitionQuery = new
                {
                    event_names = new List<string>(),
                    meter_ids = new List<string>(),
                    zone_ids = new List<int>(),
                    group_ids = new List<int>(),
                    scene_ids = new List<int>()
                };

                var query = JsonConvert.DeserializeAnonymousType(Query, definitionQuery);

                var zonesFromMeters = query?.meter_ids?.Select(x => x.Contains('_') ? x.Split('_').LastOrDefault() : x)
                    ?.SelectMany(circuit => _dsStructure.GetCircuitZones(circuit)).ToList() ?? new List<Zone>();
                var zones = (query?.zone_ids?.Select(x => (Zone)x)?.ToList() ?? new List<Zone>()).Union(zonesFromMeters).Distinct().ToList();

                filtered = events
                    .Where(x => query.event_names.Contains(x.SystemEvent.Name, StringComparer.InvariantCultureIgnoreCase))
                    .Where(x => zones.Contains(x.Properties.ZoneID))
                    .Where(x => query.group_ids.Contains(x.Properties.GroupID))
                    .Where(x => query.scene_ids.Contains(x.Properties.SceneID))
                    .ToList();
            }
            catch
            {
                filtered = events;
            }

            foreach (var dssEvent in filtered)
            {
                yield return new EventViewModel()
                {
                    Time = dssEvent.TimestampUtc,
                    Title = $"{dssEvent.SystemEvent.Name}",
                    Text = $"{dssEvent.SystemEvent.Name}, zone {(int)dssEvent.Properties.ZoneID}, group {(int)dssEvent.Properties.GroupID}, scene {(int)dssEvent.Properties.SceneID}",
                    Tags = new[] { dssEvent.SystemEvent.Name, _dsStructure.GetZoneName(dssEvent.Properties.ZoneID), dssEvent.Properties.GroupID.ToDisplayString(), dssEvent.Properties.SceneID.ToDisplayString() }
                };
            }
        }
    }
}