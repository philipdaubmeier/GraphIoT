using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class UserWiremessage : IWiremessage<UserSites>
    {
        public UserDataWiremessage? Data { get; set; }

        public UserSites? ContainedData => new UserSites()
        {
            User = Data?.Attributes,
            DefaultSiteId = Data?.Relationships?.DefaultSite?.Data?.Id,
            SiteIds = Data?.Relationships?.Sites?.Data
                ?.Where(s => s.Type?.Equals("sites", StringComparison.InvariantCultureIgnoreCase) ?? false)
                ?.Where(s => s.Id != null)?.Select(s => s.Id!)?.ToList()
        };
    }

    public class UserSites
    {
        public UserProfile? User { get; set; }
        public string? DefaultSiteId { get; set; }
        public List<string>? SiteIds { get; set; }
    }

    public class UserDataWiremessage : DataWiremessage<UserProfile>
    {
        public SiteRelationship? Relationships { get; set; }
    }

    public class SiteRelationship
    {
        public SiteRelationshipList? Sites { get; set; }

        [JsonProperty("default-site")]
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