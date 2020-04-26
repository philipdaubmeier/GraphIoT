using PhilipDaubmeier.NetatmoClient.Network;
using System;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class HomeDataResponse : Wiremessage<HomeData>
    {
        public Uri VideoUri
        {
            get
            {
                return ExtractBaseUri().Combine("/live/index.m3u8");
            }
        }

        private Uri ExtractBaseUri()
        {
            if (Body == null)
                throw new FormatException("The response message is empty.");
            if (Body.Homes.Count <= 0 || Body.Homes[0] == null)
                throw new FormatException("No home is configured in the netatmo account.");

            var home = Body.Homes[0];
            if (home.Cameras.Count <= 0 || home.Cameras[0] == null)
                throw new FormatException("No camera is configured in the netatmo account.");

            var camera = home.Cameras[0];
            if (string.IsNullOrWhiteSpace(camera.Status) || camera.Status.Trim().Equals("disconnected", StringComparison.OrdinalIgnoreCase))
                throw new FormatException("Camera is disconnected.");
            if (string.IsNullOrWhiteSpace(camera.VpnUrl))
                throw new FormatException("An empty video uri was returned.");

            try
            {
                return new Uri(camera.VpnUrl, UriKind.Absolute);
            }
            catch (Exception)
            {
                throw new FormatException("Video uri has an invalid format.");
            }
        }
    }
}