using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromHost.Structure
{
    public interface IDigitalstromStructureService
    {
        IEnumerable<Dsuid> Circuits { get; }

        IEnumerable<Zone> Zones { get; }

        IEnumerable<Zone> GetCircuitZones(Dsuid circuit);

        string GetCircuitName(Dsuid circuit);

        bool IsMeteringCircuit(Dsuid circuit);

        string GetZoneName(Zone zone);

        bool HasZoneSensor(Zone zone, Sensor type);
    }
}