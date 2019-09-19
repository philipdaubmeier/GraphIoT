namespace PhilipDaubmeier.GraphIoT.Viessmann.Config
{
    public class ViessmannConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string VitotrolDeviceId { get; set; }
        public string VitotrolInstallationId { get; set; }
        
        public string PlattformInstallationId { get; set; }
        public string PlattformGatewayId { get; set; }
        public string PlattformApiClientId { get; set; }
        public string PlattformApiClientSecret { get; set; }
    }
}