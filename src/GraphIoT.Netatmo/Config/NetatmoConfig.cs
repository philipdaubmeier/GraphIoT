namespace PhilipDaubmeier.GraphIoT.Netatmo.Config
{
    public class NetatmoConfig
    {
        public string AppId { get; set; } = null!;
        public string AppSecret { get; set; } = null!;
        public string Scope { get; set; } = null!;
        public string RedirectUri { get; set; } = null!;
    }
}