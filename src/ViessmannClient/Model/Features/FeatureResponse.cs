using Newtonsoft.Json;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class FeatureResponse<T> : IWiremessage<T> where T : FeatureList
    {
        [JsonProperty("features")]
        public T? Data { get; set; }

        public PagingCursor? Cursor => null;
    }
}