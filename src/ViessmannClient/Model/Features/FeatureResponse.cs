﻿using System.Text.Json.Serialization;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class FeatureResponse<T> : IWiremessage<T> where T : FeatureList
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        public PagingCursor? Cursor => null;
    }
}