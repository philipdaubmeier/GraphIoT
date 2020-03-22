using Newtonsoft.Json;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class FeatureResponse : IWiremessage<FeatureList>
    {
        [JsonProperty("features")]
        public FeatureList? Data { get; set; }

        public PagingCursor? Cursor => null;
    }
}