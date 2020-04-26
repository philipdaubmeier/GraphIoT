namespace PhilipDaubmeier.ViessmannClient.Model.Installations
{
    public class Address
    {
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? Zip { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FaxNumber { get; set; }
        public Geolocation? Geolocation { get; set; }
    }
}