namespace PhilipDaubmeier.GraphIoT.Viessmann.Config
{
    public class ViessmannConfig
    {
        public string InstallationId { get; set; } = null!;
        public string GatewayId { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string RedirectUri { get; set; } = null!;

        public long ParsedInstallationId => long.TryParse(InstallationId, out long id) ? id : 0;
        public long ParsedGatewayId => long.TryParse(GatewayId, out long id) ? id : 0;
    }
}