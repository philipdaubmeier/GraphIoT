namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class Administrative
    {
        public string Country { get; set; } = string.Empty;
        public string RegLocale { get; set; } = string.Empty;
        public string Lang { get; set; } = string.Empty;
        public int Unit { get; set; }
        public int Windunit { get; set; }
        public int Pressureunit { get; set; }
        public int FeelLikeAlgo { get; set; }
    }
}