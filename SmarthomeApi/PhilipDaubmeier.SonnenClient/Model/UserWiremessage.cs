using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class UserWiremessage : IWiremessage<UserSites>
    {
        public UserDataWiremessage Data { get; set; }

        public UserSites ContainedData => new UserSites()
        {
            User = Data.Attributes,
            SiteIds = Data.Relationships.Sites.Data
                .Where(s => s.Type.Equals("sites", StringComparison.InvariantCultureIgnoreCase))
                .Select(s => s.Id).ToList()
        };
    }

    public class UserSites
    {
        public UserProfile User { get; set; }
        public List<string> SiteIds { get; set; }
    }

    public class UserDataWiremessage : DataWiremessage<UserProfile>
    {
        public SiteRelationship Relationships { get; set; }
    }

    public class SiteRelationship
    {
        public SiteRelationshipList Sites { get; set; }
    }

    public class SiteRelationshipList
    {
        public LinkToRelated Links { get; set; }
        public List<Site> Data { get; set; }
    }

    public class Site
    {
        public string Type { get; set; }
        public string Id { get; set; }
    }

    public class LinkToRelated
    {
        public string Related { get; set; }
    }
}