using System.Collections.Generic;

namespace PhilipDaubmeier.TokenStore
{
    public class TokenStoreConfig
    {
        public string ConnectionString { get; set; }
        public Dictionary<string, string> ClassNameMapping { get; set; }
    }
}