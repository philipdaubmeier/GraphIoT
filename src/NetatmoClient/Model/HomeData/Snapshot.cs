namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class Snapshot
    {
        public string Id { get; set; } = string.Empty;
        public int Version { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
    }
}