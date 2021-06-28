using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class UserWiremessage : IWiremessage<UserSites>
    {
        public UserDataWiremessage? Data { get; set; }

        public UserSites ContainedData => new()
        {
            User = Data?.Attributes ?? new UserProfile(),
            DefaultSiteId = Data?.Relationships?.DefaultSite?.Data?.Id ?? string.Empty,
            SiteIds = Data?.Relationships?.Sites?.Data
                ?.Where(s => s.Type?.Equals("sites", StringComparison.InvariantCultureIgnoreCase) ?? false)
                ?.Where(s => s.Id != null)?.Select(s => s.Id!)?.ToList() ?? new List<string>()
        };
    }

    public class UserSites
    {
        public UserProfile User { get; set; } = new UserProfile();
        public string DefaultSiteId { get; set; } = string.Empty;
        public List<string> SiteIds { get; set; } = new List<string>();
    }

    public class UserDataWiremessage : DataWiremessage<UserProfile>
    {
        public SiteRelationship? Relationships { get; set; }
    }

    public class SiteRelationship
    {
        public SiteRelationshipList? Sites { get; set; }

        [JsonPropertyName("default-site")]
        public SiteRelationshipSingle? DefaultSite { get; set; }
    }

    public class SiteRelationshipSingle
    {
        public LinkToRelated? Links { get; set; }
        public Site? Data { get; set; }
    }

    public class SiteRelationshipList
    {
        public LinkToRelated? Links { get; set; }
        public List<Site>? Data { get; set; }
    }

    public class Site
    {
        public string? Type { get; set; }
        public string? Id { get; set; }
    }

    public class LinkToRelated
    {
        public string? Related { get; set; }
    }
}