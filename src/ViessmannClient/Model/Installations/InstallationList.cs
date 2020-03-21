using PhilipDaubmeier.ViessmannClient.Model.Gateways;
using System.Collections.Generic;

namespace PhilipDaubmeier.ViessmannClient.Model.Installations
{
    public class InstallationList
    {
        public List<Installation>? Data { get; set; }
        public PagingCursor? Cursor { get; set; }
    }
}