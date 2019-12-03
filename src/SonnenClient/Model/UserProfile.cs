namespace PhilipDaubmeier.SonnenClient.Model
{
    public class UserProfile
    {
        public string AcademicTitle { get; set; } = string.Empty;
        public string CustomerNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public bool Newsletter { get; set; } = false;
        public string TimeZone { get; set; } = string.Empty;
        public string PrivacyPolicy { get; set; } = string.Empty;
        public string TermsOfService { get; set; } = string.Empty;
        public bool ServicePartnersDataAccess { get; set; } = false;
    }
}