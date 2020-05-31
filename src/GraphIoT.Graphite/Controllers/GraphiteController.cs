using Microsoft.AspNetCore.Mvc;
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
using System.Text.Json;

namespace PhilipDaubmeier.GraphIoT.Grafana.Controllers
{
    /// <summary>
    /// Emulates the graphite api, to be able to consume GraphIoT in Grafana with the Graphite data source
    /// </summary>
    [Produces("application/json")]
    [Route("api/graphite")]
    public class GraphiteController : Controller
    {
        private readonly GraphDataSource _dataSource;
        private readonly IList<IEventCollectionViewModel> _eventViewModels;

        public GraphiteController(IEnumerable<IGraphCollectionViewModel> graphViewModels, IEnumerable<IEventCollectionViewModel> eventViewModels)
        {
            _dataSource = new GraphDataSource(graphViewModels);
            _eventViewModels = eventViewModels.ToList();
        }

        // POST: api/graphite/render
        [HttpPost("render")]
        public ActionResult Render([FromForm] IEnumerable<string> target, [FromForm] string from, [FromForm] string until, [FromForm] string format, [FromForm] string maxDataPoints)
        {
            if (!from.TryParseGraphiteTime(out DateTime fromDate)
                || !until.TryParseGraphiteTime(out DateTime toDate)
                || !int.TryParse(maxDataPoints, out int count))
                return StatusCode((int)HttpStatusCode.OK);

            if (!format.Equals("json", StringComparison.InvariantCultureIgnoreCase))
                return StatusCode((int)HttpStatusCode.BadRequest);

            _dataSource.Span = new TimeSeriesSpan(fromDate, toDate, count);
            var parser = new Parser() { DataSource = _dataSource };
            return Json(target.SelectMany(t => parser.Parse(t).Graphs).Select(g => new
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
            return Json(_dataSource.AutocompletePath(query).Select(g => new { g.expandable, text = g.segment }));
        }

        // GET: api/graphite/tags/autoComplete/tags
        [HttpGet("tags/autoComplete/tags")]
        public ActionResult GetTags()
        {
            return Json(new string[] { });
        }

        // GET: api/graphite/functions
        [HttpGet("functions")]
        public ActionResult GetFunctions()
        {
            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return Content('{' + string.Join(',', GraphiteFunctionMap.GetFunctions()
                .Select(f => $"\"{f.Name}\":" + JsonSerializer.Serialize(f, options))) + '}', "application/json");
        }

        // GET: api/graphite/events/get_data
        // Grafana calls it like this: events/get_data?from=-2d&until=now&tags=mytag
        [HttpGet("events/get_data")]
        public ActionResult GetEvents(string tags, string from, string until)
        {
            if (!from.TryParseGraphiteTime(out DateTime fromDate)
                || !until.TryParseGraphiteTime(out DateTime toDate))
                return StatusCode((int)HttpStatusCode.BadRequest);

            var eventViewModel = _eventViewModels.FirstOrDefault();
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