namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class User
    {
        public string Mail { get; set; } = string.Empty;
        public Administrative Administrative { get; set; } = new Administrative();
    }
}