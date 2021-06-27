using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Aggregation;
using PhilipDaubmeier.GraphIoT.Core.Parsers;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Grafana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Grafana.Controllers
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
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private List<string>? graphIds = null;
        public List<string> GraphIds
        {
            get
            {
                static string ToRawId(string name) => Regex.Replace(name.ToLowerInvariant().Replace("ä", "ae").Replace("ö", "oe")
                    .Replace("ü", "ue").Replace("ß", "ss"), @"[^\u0000-\u007F]+", string.Empty).Replace(' ', '_');

                if (graphIds is null)
                {
                    graphIds = graphViewModels
                        .SelectMany(n => n.Value.Graphs()
                            .Zip(Enumerable.Range(0, 100), (g, i) => new Tuple<int, string, string>(i, g.Name, g.Key))
                            .SelectMany(t => new string[]
                            {
                                t.Item2,
                                $"{n.Key}_{t.Item1}_{ToRawId(t.Item3)}"
                            })
                        ).ToList();
                }
                return graphIds;
            }
        }

        private readonly Dictionary<string, IGraphCollectionViewModel> graphViewModels;
        private readonly Dictionary<string, IEventCollectionViewModel> eventViewModels;

        public GrafanaController(IEnumerable<IGraphCollectionViewModel> graphViewModels, IEnumerable<IEventCollectionViewModel> eventViewModels)
        {
            this.graphViewModels = graphViewModels.ToDictionary(x => x.Key, x => x);
            this.eventViewModels = eventViewModels.ToDictionary(x => x.Key, x => x);
        }

        // GET: api/grafana/
        [HttpGet]
        public ActionResult TestConnection()
        {
            return StatusCode((int)HttpStatusCode.OK);
        }

        // POST: api/grafana/search
        [HttpPost("search")]
        public ActionResult Search()
        {
            return Json(GraphIds.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct());
        }

        // POST: api/grafana/query
        [HttpPost("query")]
        public async Task<ActionResult> Query()
        {
            var query = await JsonSerializer.DeserializeAsync<GrafanaQuery>(Request.Body, _jsonSerializerOptions);

            if (query is null || !DateTime.TryParse(query.Range.From, out DateTime fromDate) || !DateTime.TryParse(query.Range.To, out DateTime toDate))
                return StatusCode((int)HttpStatusCode.NotFound);

            var span = new TimeSeriesSpan(fromDate.ToUniversalTime(), toDate.ToUniversalTime(), query.MaxDataPoints);

            var data = new Dictionary<string, List<dynamic[]>>();
            foreach (var target in query.Targets)
            {
                var targetId = target.Data?.RawMetricId ?? target?.Target;
                if (string.IsNullOrEmpty(targetId))
                    continue;

                // filter if the filterIfNoneOf list is present but the target is not contained
                var filterIfNoneOf = target?.Data?.FilterIfNoneOf;
                if (filterIfNoneOf != null && !filterIfNoneOf.Contains(targetId))
                    continue;

                // find the parent viewmodel of the target graph
                var splitted = targetId.Split('_');
                if (splitted.Length < 2 || !int.TryParse(splitted[1], out int index) || index < 0
                    || !graphViewModels.ContainsKey(splitted[0]) || index >= graphViewModels[splitted[0]].GraphCount())
                    continue;
                var viewModel = graphViewModels[splitted[0]];

                // if a custom 'aggregate.interval' or 'overrideMaxDataPoints' was given, take that as time spacing
                var targetSpan = span;
                var spanOverride = target?.Data?.Aggregate?.Interval?.ToTimeSpan();
                if (spanOverride.HasValue && spanOverride.Value > span.Duration)
                    targetSpan = new TimeSeriesSpan(fromDate, toDate, spanOverride.Value);
                else if (target?.Data?.OverrideMaxDataPoints.HasValue ?? false)
                    targetSpan = new TimeSeriesSpan(fromDate, toDate, target?.Data?.OverrideMaxDataPoints ?? 1);
                viewModel.Span = targetSpan;

                // if a custom 'aggregate.func' was given, take that as aggregation method
                static Aggregator ToAggregator(string? aggregateRaw)
                {
                    return (aggregateRaw?.Substring(0, Math.Min(3, aggregateRaw.Length))?.ToLowerInvariant()) switch
                    {
                        "min" => Aggregator.Minimum,
                        "max" => Aggregator.Maximum,
                        "avg" => Aggregator.Average,
                        "sum" => Aggregator.Sum,
                        _ => Aggregator.Default,
                    };
                }
                viewModel.AggregatorFunction = ToAggregator(target?.Data?.Aggregate?.Func);

                // if a custom correction factor and/or offset was given, set them to get them taken into account
                viewModel.CorrectionFactor = target?.Data?.Correction?.Factor ?? 1d;
                viewModel.CorrectionOffset = target?.Data?.Correction?.Offset ?? 0d;

                // add the resampled target graph to the result
                data.Add(target?.Target ?? targetId, viewModel.Graph(index).TimestampedPoints().ToList());
            }

            return Json(data.Select(d => new { target = d.Key, datapoints = d.Value }));
        }

        // POST: api/grafana/annotations
        [HttpPost("annotations")]
        public async Task<ActionResult> AnnotationsAsync()
        {
            var annotationInfo = await JsonSerializer.DeserializeAsync<AnnotationQuery>(Request.Body, _jsonSerializerOptions);

            if (annotationInfo is null || !DateTime.TryParse(annotationInfo.Range.From, out DateTime fromDate) || !DateTime.TryParse(annotationInfo.Range.To, out DateTime toDate))
                return StatusCode((int)HttpStatusCode.NotFound);

            var eventViewModel = eventViewModels.FirstOrDefault().Value;
            eventViewModel.Span = new TimeSeriesSpan(fromDate, toDate, 1);
            eventViewModel.Query = annotationInfo.Annotation.Query;

            return Json(eventViewModel.Events.Select(item => new
            {
                annotation = annotationInfo.Annotation.Name,
                time = new DateTimeOffset(item.Time.ToUniversalTime()).ToUnixTimeMilliseconds(),
                title = item.Title,
                tags = item.Tags,
                text = item.Text
            }));
        }
    }
}