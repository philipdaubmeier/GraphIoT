using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class ChargerWiremessage : IWiremessage<List<Charger>>
    {
        public List<DataWiremessage<Charger>>? Data { get; set; }

        public List<Charger> ContainedData => Data?.Where(d => d.Attributes != null)?.Select(d => {
            if (d.Attributes != null)
                d.Attributes.Id = d.Id ?? string.Empty;
            return d.Attributes!;
            })?.ToList() ?? new List<Charger>();
    }
}