namespace PhilipDaubmeier.GraphIoT.Netatmo.Config
{
    public class NetatmoConfig
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string AppId { get; set; } = null!;
        public string AppSecret { get; set; } = null!;
        public string Scope { get; set; } = null!;
    }
}