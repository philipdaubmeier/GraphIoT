﻿using Microsoft.Extensions.Options;
using PhilipDaubmeier.TokenStore;
using PhilipDaubmeier.ViessmannClient.Network;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Config
{
    public class ViessmannHttpClient<T>
    {
        public ViessmannHttpClient(HttpClient client) => Client = client;
        public HttpClient Client { get; private set; }
    }

    public class ViessmannConfigConnectionProvider<T> : ViessmannConnectionProvider<T>
    {
        public ViessmannConfigConnectionProvider(TokenStore<T> tokenStore, IOptions<ViessmannConfig> config, ViessmannHttpClient<T> client)
            : base(new ViessmannAuth<T>(tokenStore))
        {
            ClientId = config.Value.ClientId;
            RedirectUri = config.Value.RedirectUri;

            Client = client.Client;
        }
    }
}