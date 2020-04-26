using Microsoft.Extensions.Options;
using PhilipDaubmeier.GraphIoT.Viessmann.Config;
using PhilipDaubmeier.ViessmannClient.Network;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PhilipDaubmeier.ViessmannClient
{
    public class ViessmannVitotrolClient
    {
        private readonly IViessmannConnectionProvider<ViessmannVitotrolClient> _connectionProvider;
        private readonly ViessmannConfig _config;

        private readonly HttpClient _client;

        private const string _soapUri = @"https://api.viessmann.io/vitotrol/soap/v1.0/iPhoneWebService.asmx";
        private const string _soapPrefix = @"<?xml version=""1.0"" encoding=""UTF-8""?><soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns=""http://www.e-controlnet.de/services/vii/""><soap:Body>";
        private const string _soapSuffix = @"</soap:Body></soap:Envelope>";
        private const string _soapAction = @"http://www.e-controlnet.de/services/vii/";

        public ViessmannVitotrolClient(IViessmannConnectionProvider<ViessmannVitotrolClient> connectionProvider, IOptions<ViessmannConfig> config)
        {
            _connectionProvider = connectionProvider;
            _client = _connectionProvider.Client;
            _config = config.Value;
        }

        public async Task<List<(string id, string name)>> GetTypeInfo()
        {
            await Authenticate();

            var body = $"<GeraetId>{_config.VitotrolDeviceId}</GeraetId><AnlageId>{_config.VitotrolInstallationId}</AnlageId>";
            return await ParseTypeInfo(await SendSoap("GetTypeInfo", body, _connectionProvider.AuthData.AccessToken));
        }

        public async Task<List<(string id, string value, DateTime timestamp)>> GetData(IEnumerable<int> datapoints)
        {
            await Authenticate();

            var datapointList = string.Join("</int><int>", datapoints.Select(x => x.ToString()));
            var body = $"<UseCache>false</UseCache><GeraetId>{_config.VitotrolDeviceId}</GeraetId><AnlageId>{_config.VitotrolInstallationId}</AnlageId><DatenpunktIds><int>{datapointList}</int></DatenpunktIds>";
            return await ParseData(await SendSoap("GetData", body, _connectionProvider.AuthData.AccessToken));
        }

        private async Task Authenticate()
        {
            if (_connectionProvider.AuthData.IsAccessTokenValid())
                return;

            var body = $"<Betriebssystem>Android</Betriebssystem><AppId>prod</AppId><Benutzer>{_connectionProvider.AuthData.Username}</Benutzer><AppVersion>93</AppVersion><Passwort>{_connectionProvider.AuthData.UserPassword}</Passwort>";
            var response = await SendSoap("Login", body, null);

            var cookies = response.Headers.GetValues("Set-Cookie").Select(x => x.Replace("path=/; HttpOnly", "").Trim().TrimEnd(';')).ToList();
            var sessiontoken = string.Join(";", cookies);

            await _connectionProvider.AuthData.UpdateTokenAsync(sessiontoken, DateTime.Now.AddHours(1), string.Empty);
        }

        private async Task<HttpResponseMessage> SendSoap(string action, string body, string? token)
        {
            var soapBody = $"{_soapPrefix}<{action}>{body}</{action}>{_soapSuffix}";
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_soapUri));
            request.Headers.Add("SOAPAction", $"{_soapAction}{action}");
            request.Headers.Add("Connection", "Keep-Alive");
            request.Headers.TryAddWithoutValidation("User-Agent", "Dalvik/2.1.0 (Linux; U; Android 9; Nokia 8 Sirocco Build/PPR1.180610.011)");
            request.Headers.Host = "api.viessmann.io";
            if (token != null)
                request.Headers.Add("Cookie", token);
            request.Content = new StringContent(soapBody, Encoding.UTF8, "text/xml");

            return await _client.SendAsync(request);
        }

        private async Task<List<(string id, string name)>> ParseTypeInfo(HttpResponseMessage response)
        {
            var elem = await XElement.LoadAsync(await response.Content.ReadAsStreamAsync(), LoadOptions.None, new CancellationToken());
            return elem.Descendants().First(x => x.Name.LocalName.Equals("TypeInfoListe", StringComparison.InvariantCultureIgnoreCase))
                .Descendants().Where(x => x.Name.LocalName.Equals("DatenpunktTypInfo", StringComparison.InvariantCultureIgnoreCase)).Select(d =>
            {
                return (d.Descendants().First(x => x.Name.LocalName.Equals("DatenpunktId", StringComparison.InvariantCultureIgnoreCase)).Value,
                        d.Descendants().First(x => x.Name.LocalName.Equals("DatenpunktName", StringComparison.InvariantCultureIgnoreCase)).Value);
            }).ToList();
        }

        private async Task<List<(string id, string value, DateTime timestamp)>> ParseData(HttpResponseMessage response)
        {
            var elem = await XElement.LoadAsync(await response.Content.ReadAsStreamAsync(), LoadOptions.None, new CancellationToken());
            return elem.Descendants().First(x => x.Name.LocalName.Equals("DatenwerteListe", StringComparison.InvariantCultureIgnoreCase))
                .Descendants().Where(x => x.Name.LocalName.Equals("WerteListe", StringComparison.InvariantCultureIgnoreCase)).Select(d =>
                {
                    return (d.Descendants().First(x => x.Name.LocalName.Equals("DatenpunktId", StringComparison.InvariantCultureIgnoreCase)).Value,
                            d.Descendants().First(x => x.Name.LocalName.Equals("Wert", StringComparison.InvariantCultureIgnoreCase)).Value,
                            DateTime.ParseExact(
                                d.Descendants().First(x => x.Name.LocalName.Equals("Zeitstempel", StringComparison.InvariantCultureIgnoreCase)).Value,
                                "yyyy'-'MM'-'dd HH':'mm':'ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));
                }).ToList();
        }
    }
}
