using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.User
{
    internal class UserProfileResponse
    {
        [JsonPropertyName("sub")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("given_name")]
        public string GivenName { get; set; } = string.Empty;

        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; } = false;

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("phone_number_verified")]
        public bool PhoneNumberVerified { get; set; } = false;

        [JsonPropertyName("address")]
        public UserAddress Address { get; set; } = new();

        [JsonPropertyName("updated_at")]
        public int UpdatedAt { get; set; } = 0;

        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;
    }

    internal class UserAddress
    {
        [JsonPropertyName("street_address")]
        public string StreetAddress { get; set; } = string.Empty;

        [JsonPropertyName("locality")]
        public string Locality { get; set; } = string.Empty;

        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("formatted")]
        public string Formatted { get; set; } = string.Empty;
    }
}