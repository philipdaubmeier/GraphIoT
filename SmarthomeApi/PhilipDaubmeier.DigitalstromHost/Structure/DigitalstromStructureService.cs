using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PhilipDaubmeier.DigitalstromHost.Structure
{
    public class DigitalstromStructureService : IDigitalstromStructureService
    {
        private List<Dsuid> circuits = null;
        private List<Zone> zones = null;
        private Dictionary<Dsuid, List<Zone>> circuitZones = null;
        private Dictionary<Dsuid, string> circuitNames = null;
        private Dictionary<Zone, string> zoneNames = null;

        private readonly DigitalstromDssClient dsClient;

        private readonly Semaphore _loadSemaphore = new Semaphore(1, 1);

        public DigitalstromStructureService(DigitalstromDssClient digitalstromClient)
        {
            dsClient = digitalstromClient;
        }

        public IEnumerable<Dsuid> Circuits
        {
            get
            {
                LazyLoad();
                return circuits;
            }
        }

        public IEnumerable<Zone> Zones
        {
            get
            {
                LazyLoad();
                return zones;
            }
        }

        public IEnumerable<Zone> GetCircuitZones(Dsuid circuit)
        {
            LazyLoad();

            if (circuitZones.ContainsKey(circuit))
                foreach (var zone in circuitZones[circuit])
                    yield return zone;

            yield break;
        }

        public string GetCircuitName(Dsuid circuit)
        {
            LazyLoad();

            if (circuitNames.TryGetValue(circuit, out string name))
                return name;

            return circuit.ToString();
        }

        public string GetZoneName(Zone zone)
        {
            LazyLoad();

            if (zoneNames.TryGetValue(zone, out string name))
                return name;

            return zone.ToString();
        }

        private void LazyLoad()
        {
            try
            {
                _loadSemaphore.WaitOne();

                if (circuitZones != null && circuitNames != null && zoneNames != null)
                    return;

                var circuitZoneRes = dsClient.GetCircuitZones().Result;
                circuitZones = circuitZoneRes.DSMeters.ToDictionary(x => x.DSUID, x => x?.Zones?.Select(y => y.ZoneID)?.ToList() ?? new List<Zone>());

                var circuitNameRes = dsClient.GetMeteringCircuits().Result;
                circuitNames = circuitNameRes.DSMeters.ToDictionary(x => x.DSUID, x => x.Name);

                var zoneNameRes = dsClient.GetStructure().Result;
                zoneNames = zoneNameRes.Zones.ToDictionary(x => x.Id, x => x.Name);

                circuits = circuitZones.Keys.Union(circuitNames.Keys).Distinct().OrderBy(x => (string)x).ToList();
                zones = zoneNames.Keys.Union(circuitZones.SelectMany(x => x.Value)).Distinct().OrderBy(x => (int)x).ToList();
            }
            finally { _loadSemaphore.Release(); }
        }
    }
}