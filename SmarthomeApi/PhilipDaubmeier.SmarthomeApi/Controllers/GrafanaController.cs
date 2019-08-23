using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NodaTime;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromHost.ViewModel;
using PhilipDaubmeier.SmarthomeApi.Database;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;
using PhilipDaubmeier.ViessmannHost.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    /// <summary>
    /// More documentation about datasource plugins can be found in the Docs:
    /// https://github.com/grafana/grafana/blob/master/docs/sources/plugins/developing/datasources.md
    /// 
    /// A grafana json data source backend needs to implement 4 urls:
    /// "/" should return 200 ok.Used for "Test connection" on the datasource config page.
    /// "/search" used by the find metric options on the query tab in panels.
    /// "/query" should return metrics based on input.
    /// "/annotations" should return annotations.
    /// 
    /// Those two urls are optional:
    /// 
    /// "/tag-keys" should return tag keys for ad hoc filters.
    /// "/tag-values" should return tag values for ad hoc filters.
    /// </summary>
    [Produces("application/json")]
    [Route("api/grafana")]
    public class GrafanaController : Controller
    {
        private List<string> graphIds = null;
        public List<string> GraphIds
        {
            get
            {
                if (graphIds == null)
                {
                    graphIds = GenerateViewModels(new TimeSeriesSpan(DateTime.Now.AddDays(-1), DateTime.Now, 1))
                        .SelectMany(n => n.Value.Graphs().Zip(Enumerable.Range(0, 100), (g, i) => new Tuple<int, string>(i, g.Name.ToLowerInvariant()))
                        .Select(t => new Tuple<int, string>(t.Item1, t.Item2.Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue").Replace("ß", "ss")))
                        .Select(t => new Tuple<int, string>(t.Item1, Regex.Replace(t.Item2, @"[^\u0000-\u007F]+", string.Empty)))
                        .Select(t => $"{n.Key}_{t.Item1}_{t.Item2.Replace(' ', '_')}")).ToList();
                }
                return graphIds;
            }
        }

        private static Dictionary<Dsuid, List<Zone>> circuitZones = null;

        private readonly PersistenceContext db;
        private readonly DigitalstromDssClient dsClient;
        public GrafanaController(PersistenceContext databaseContext, DigitalstromDssClient digitalstromClient)
        {
            db = databaseContext;
            dsClient = digitalstromClient;
        }

        private Dictionary<string, IGraphCollectionViewModel> GenerateViewModels(TimeSeriesSpan span)
        {
            return new Dictionary<string, IGraphCollectionViewModel>()
            {
                { "energy", new DigitalstromEnergyViewModel(db, span) },
                { "sensors", new DigitalstromZoneSensorViewModel(db, span) },
                { "heating", new ViessmannHeatingViewModel(db, span) },
                { "solar", new ViessmannSolarViewModel(db, span) },
                { "solarenergy", new SonnenEnergyViewModel(db, span) },
            };
        }

        // GET: api/grafana/
        [HttpGet]
        public ActionResult TestConnection()
        {
            return StatusCode(200);
        }

        // POST: api/grafana/search
        [HttpPost("search")]
        public ActionResult Search()
        {
            return Json(GraphIds);
        }

        // POST: api/grafana/query
        [HttpPost("query")]
        public async Task<ActionResult> Query()
        {
            var definition = new
            {
                panelId = 0,
                range = new
                {
                    from = "",
                    to = "",
                    raw = new
                    {
                        from = "",
                        to = ""
                    }
                },
                rangeRaw = new
                {
                    from = "",
                    to = ""
                },
                interval = "",
                intervalMs = 0,
                targets = new[] { new
                {
                    target = "",
                    refId = "",
                    type = "timeserie"
                } },
                adhocFilters = new[] { new
                {
                    key = "",
                    @operator = "=",
                    value = ""
                } },
                format = "json",
                maxDataPoints = 0
            };

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            var query = JsonConvert.DeserializeAnonymousType(body, definition);

            if (!DateTime.TryParse(query.range.from, out DateTime fromDate) || !DateTime.TryParse(query.range.to, out DateTime toDate))
                return StatusCode(404);

            var span = new TimeSeriesSpan(fromDate, toDate, query.maxDataPoints);
            var viewModels = GenerateViewModels(span);

            var data = new Dictionary<string, List<dynamic[]>>();
            foreach (var target in query.targets)
            {
                if (target == null || string.IsNullOrEmpty(target.target))
                    continue;

                var splitted = target.target.Split('_');
                if (splitted.Length < 2 || !int.TryParse(splitted[1], out int index) || index < 0
                    || !viewModels.ContainsKey(splitted[0]) || index >= viewModels[splitted[0]].GraphCount())
                    continue;

                data.Add(target.target, viewModels[splitted[0]].Graph(index).TimestampedPoints().ToList());
            }

            return Json(data.Select(d => new { target = d.Key, datapoints = d.Value }));
        }

        // POST: api/grafana/annotations
        [HttpPost("annotations")]
        public async Task<ActionResult> AnnotationsAsync()
        {
            var definition = new
            {
                range = new
                {
                    from = "",
                    to = ""
                },
                rangeRaw = new
                {
                    from = "",
                    to = ""
                },
                annotation = new
                {
                    name = "",
                    datasource = "",
                    iconColor = "",
                    enable = true,
                    query = ""
                }
            };

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            var annotationInfo = JsonConvert.DeserializeAnonymousType(body, definition);

            if (!DateTime.TryParse(annotationInfo.range.from, out DateTime fromDate) || !DateTime.TryParse(annotationInfo.range.to, out DateTime toDate))
                return StatusCode(404);

            var eventData = db.DsSceneEventDataSet.Where(x => x.Day >= fromDate.Date && x.Day <= toDate.Date);
            var events = eventData.SelectMany(x => x.EventStream).Where(x => x.TimestampUtc >= fromDate.ToUniversalTime() && x.TimestampUtc <= toDate.ToUniversalTime()).ToList();
            List<DssEvent> filtered;

            try
            {
                var definitionQuery = new
                {
                    event_names = new List<string>(),
                    meter_ids = new List<string>(),
                    zone_ids = new List<int>(),
                    group_ids = new List<int>(),
                    scene_ids = new List<int>()
                };

                var query = JsonConvert.DeserializeAnonymousType(annotationInfo.annotation.query, definitionQuery);

                if (circuitZones == null)
                {
                    var res = dsClient.GetCircuitZones().Result;
                    circuitZones = res.DSMeters.ToDictionary(x => x.DSUID, x => x?.Zones?.Select(y => y.ZoneID)?.ToList() ?? new List<Zone>());
                }

                var zonesFromMeters = query?.meter_ids?.Select(x => x.Contains('_') ? x.Split('_').LastOrDefault() : x)
                    ?.SelectMany(x => circuitZones.ContainsKey(x) ? circuitZones[x] : new List<Zone>()).ToList() ?? new List<Zone>();
                var zones = (query?.zone_ids?.Cast<Zone>()?.ToList() ?? new List<Zone>()).Union(zonesFromMeters).Distinct().ToList();

                filtered = events
                    .Where(x => query.event_names.Contains(x.SystemEvent.Name, StringComparer.InvariantCultureIgnoreCase))
                    .Where(x => zones.Contains(x.Properties.ZoneID))
                    .Where(x => query.group_ids.Contains(x.Properties.GroupID))
                    .Where(x => query.scene_ids.Contains(x.Properties.SceneID))
                    .ToList();
            }
            catch
            {
                filtered = events;
            }
            
            return Json(filtered.Select(dssEvent => new
            {
                annotation = annotationInfo.annotation.name,
                time = Instant.FromDateTimeUtc(dssEvent.TimestampUtc).ToUnixTimeMilliseconds(),
                title = $"{dssEvent.SystemEvent.Name}",
                tags = new[] { dssEvent.SystemEvent.Name, dssEvent.Properties.GroupID.ToString(), dssEvent.Properties.SceneID.ToDisplayString() },
                text = $"{dssEvent.SystemEvent.Name}, zone {(int)dssEvent.Properties.ZoneID}, group {(int)dssEvent.Properties.GroupID}, scene {(int)dssEvent.Properties.SceneID}"
            }));
        }
    }
}