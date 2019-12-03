namespace PhilipDaubmeier.GraphIoT.Viessmann.Config
{
    public class ViessmannConfig
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        public string VitotrolDeviceId { get; set; } = null!;
        public string VitotrolInstallationId { get; set; } = null!;

        public string PlattformInstallationId { get; set; } = null!;
        public string PlattformGatewayId { get; set; } = null!;
        public string PlattformApiClientId { get; set; } = null!;
        public string PlattformApiClientSecret { get; set; } = null!;
    }
}