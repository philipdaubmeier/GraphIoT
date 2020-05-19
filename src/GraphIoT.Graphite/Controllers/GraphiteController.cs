using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Graphite.Model;
using PhilipDaubmeier.GraphIoT.Graphite.Parser;
using PhilipDaubmeier.GraphIoT.Graphite.Parser.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Grafana.Controllers
{
    /// <summary>
    /// Emulates the graphite api, to be able to consume GraphIoT in Grafana with the Graphite data source
    /// </summary>
    [Produces("application/json")]
    [Route("api/graphite")]
    public class GraphiteController : Controller
    {
        private readonly GraphDataSource dataSource;
        private readonly Dictionary<string, IEventCollectionViewModel> eventViewModels;

        public GraphiteController(IEnumerable<IGraphCollectionViewModel> graphViewModels, IEnumerable<IEventCollectionViewModel> eventViewModels)
        {
            dataSource = new GraphDataSource(graphViewModels);
            this.eventViewModels = eventViewModels.ToDictionary(x => x.Key, x => x);
        }

        // POST: api/graphite/render
        [HttpPost("render")]
        public async Task<ActionResult> Render()
        {
            var formData = await Request.ReadFormAsync();
            if (!formData.TryGetValue("target", out StringValues targets)
                || !formData.TryGetValue("from", out StringValues fromRaw)
                || !formData.TryGetValue("until", out StringValues untilRaw)
                || !formData.TryGetValue("format", out StringValues formatRaw)
                || !formData.TryGetValue("maxDataPoints", out StringValues maxPointsRaw)
                || !fromRaw.FirstOrDefault().TryParseGraphiteTime(out DateTime from)
                || !untilRaw.FirstOrDefault().TryParseGraphiteTime(out DateTime until)
                || !int.TryParse(maxPointsRaw.FirstOrDefault(), out int maxDataPoints))
                return StatusCode((int)HttpStatusCode.OK);

            if (!(formatRaw.FirstOrDefault()?.Equals("json", StringComparison.InvariantCultureIgnoreCase) ?? false))
                return StatusCode((int)HttpStatusCode.BadRequest);

            dataSource.Span = new TimeSeriesSpan(from.ToUniversalTime(), until.ToUniversalTime(), maxDataPoints);

            var parser = new Parser() { DataSource = dataSource };
            return Json(targets.SelectMany(target => parser.Parse(target).Graphs).Select(g => new
            {
                target = g.Name,
                datapoints = g.TimestampedPoints()
            }));
        }

        // GET: api/graphite/metrics/find
        // Grafana calls it like this: metrics/find?from=1589353272&query=*&until=1589526192
        [HttpGet("metrics/find")]
        [SuppressMessage("Style", "IDE0060", Justification = "'from' and 'until' are passed in by grafana but not used to exclude graphs.")]
        public ActionResult FindMetrics(int from, int until, string query)
        {
            var q = new TargetQuery(query);

            return Json(dataSource.GraphKeys.Where(g => q.IsMatch(g)).Select(g => q.NextSegment(g))
                .Select(g => new { g.expandable, text = g.segment }).Distinct());
        }

        // GET: api/graphite/tags/autoComplete/tags
        [HttpGet("tags/autoComplete/tags")]
        public ActionResult GetTags()
        {
            return Json(new string[] { });
        }

        // GET: api/graphite/events/get_data
        // Grafana calls it like this: events/get_data?from=-2d&until=now&tags=mytag
        [HttpGet("events/get_data")]
        public ActionResult GetEvents(string tags, string from, string until)
        {
            if (!from.TryParseGraphiteTime(out DateTime fromDate)
                || !until.TryParseGraphiteTime(out DateTime toDate))
                return StatusCode((int)HttpStatusCode.BadRequest);

            var eventViewModel = eventViewModels.FirstOrDefault().Value;
            eventViewModel.Span = new TimeSeriesSpan(fromDate, toDate, 1);
            eventViewModel.Query = tags;

            var counter = 0;
            return Json(eventViewModel.Events.Select(item => new
            {
                id = counter++,
                what = item.Title,
                when = new DateTimeOffset(item.Time.ToUniversalTime()).ToUnixTimeMilliseconds(),
                tags = item.Tags,
                data = item.Text
            }));
        }
    }
}