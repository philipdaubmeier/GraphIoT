using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Emanager
{
    internal class EmanagerResponse : Wiremessage<Emanager>
    {
        [JsonPropertyName("EManager")]
        public override Emanager Body { get; set; } = new Emanager();
    }
}