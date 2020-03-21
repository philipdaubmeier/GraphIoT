using System.Collections.Generic;

namespace PhilipDaubmeier.ViessmannClient.Model.Gateways
{
    public class GatewayList
    {
        public List<Gateway>? Data { get; set; }
        public PagingCursor? Cursor { get; set; }
    }
}