﻿using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Structure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.ViewModel
{
    public class DigitalstromSceneEventViewModel : EventCollectionViewModel
    {
        private class EventQuery
        {
            [JsonPropertyName("event_names")]
            public List<string>? EventNames { get; set; }

            [JsonPropertyName("meter_ids")]
            public List<string>? MeterIds { get; set; }

            [JsonPropertyName("zone_ids")]
            public List<int>? ZoneIds { get; set; }

            [JsonPropertyName("group_ids")]
            public List<int>? GroupIds { get; set; }

            [JsonPropertyName("scene_ids")]
            public List<int>? SceneIds { get; set; }
        };

        private readonly IDigitalstromDbContext _db;
        private readonly IDigitalstromStructureService _dsStructure;
        private readonly IStringLocalizer<DigitalstromSceneEventViewModel> _localizer;
        private readonly ILogger _logger;

        public DigitalstromSceneEventViewModel(IDigitalstromDbContext databaseContext, IDigitalstromStructureService dsStructure, IStringLocalizer<DigitalstromSceneEventViewModel> localizer, ILogger<DigitalstromSceneEventViewModel> logger) : base()
        {
            _db = databaseContext;
            _dsStructure = dsStructure;
            _localizer = localizer;
            _logger = logger;
        }

        public override string Key => "sceneevents";

        public override IEnumerable<EventViewModel> Events => EnumerateEvents();

        private IEnumerable<EventViewModel> EnumerateEvents()
        {
            if (Span is null || Query is null)
                yield break;

            var eventData = _db.DsSceneEventDataSet.Where(x => x.Key >= Span.Begin.Date && x.Key <= Span.End.Date).ToList();
            var events = eventData.SelectMany(x => x.EventStream).Where(x => x.TimestampUtc >= Span.Begin.ToUniversalTime() && x.TimestampUtc <= Span.End.ToUniversalTime()).ToList();
            List<DssEvent> filtered;

            try
            {
                var query = JsonSerializer.Deserialize<EventQuery>(Query);

                var zonesFromMeters = query?.MeterIds?.Select(x => x.Contains('_') ? x.Split('_').LastOrDefault() : x)
                    ?.SelectMany(circuit => _dsStructure.GetCircuitZones(circuit)).ToList() ?? new List<Zone>();
                var zones = (query?.ZoneIds?.Select(x => (Zone)x)?.ToList() ?? new List<Zone>()).Union(zonesFromMeters).Distinct().ToList();

                filtered = events
                    .Where(x => query?.EventNames?.Contains(x.SystemEvent.Name, StringComparer.InvariantCultureIgnoreCase) ?? true)
                    .Where(x => zones.Contains(x.Properties.ZoneID))
                    .Where(x => query?.GroupIds?.Contains(x.Properties.GroupID) ?? true)
                    .Where(x => query?.SceneIds?.Contains(x.Properties.SceneID) ?? true)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Annotation Query error, using unfiltered event list. Message: {ex.Message}");

                filtered = events;
            }

            foreach (var dssEvent in filtered)
            {
                yield return new EventViewModel(
                    time: dssEvent.TimestampUtc,
                    title: (string?)_localizer["event: {0}", dssEvent.SystemEvent.Name] ?? string.Empty,
                    text: (string?)_localizer["{0}, zone {1}, group {2}, scene {3}", dssEvent.SystemEvent.Name, (int)dssEvent.Properties.ZoneID, (int)dssEvent.Properties.GroupID, (int)dssEvent.Properties.SceneID] ?? string.Empty,
                    tags: new[] { dssEvent.SystemEvent.Name, _dsStructure.GetZoneName(dssEvent.Properties.ZoneID), dssEvent.Properties.GroupID.ToString("d", CultureInfo.CurrentUICulture), dssEvent.Properties.SceneID.ToString("d", CultureInfo.CurrentUICulture) }
                );
            }
        }
    }
}