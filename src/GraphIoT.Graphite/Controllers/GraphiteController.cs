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

        public GraphiteController(IEnumerable<IGraphCollectionViewModel> graphViewModels, IEnumerable<IEventCollectionViewModel> eventViewModels)
        {
            dataSource = new GraphDataSource(graphViewModels);
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
    }
}