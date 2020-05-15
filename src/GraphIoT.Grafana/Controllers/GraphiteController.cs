using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
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
        private List<string>? graphIds = null;
        public List<string> GraphIds
        {
            get
            {
                if (graphIds is null)
                    graphIds = graphViewModels.SelectMany(n => n.Value.Graphs().Select(g => $"{n.Key}.{g.Key}".Replace('_', '.'))).ToList();

                return graphIds;
            }
        }

        private readonly Dictionary<string, IGraphCollectionViewModel> graphViewModels;
        private readonly Dictionary<string, IEventCollectionViewModel> eventViewModels;

        public GraphiteController(IEnumerable<IGraphCollectionViewModel> graphViewModels, IEnumerable<IEventCollectionViewModel> eventViewModels)
        {
            this.graphViewModels = graphViewModels.ToDictionary(x => x.Key, x => x);
            this.eventViewModels = eventViewModels.ToDictionary(x => x.Key, x => x);
        }

        // POST: api/graphite/render
        [HttpPost("render")]
        public async Task<ActionResult> Render()
        {
            var formData = await Request.ReadFormAsync();
            if (!formData.TryGetValue("target", out StringValues targetRaw)
                || !formData.TryGetValue("from", out StringValues fromRaw)
                || !formData.TryGetValue("until", out StringValues untilRaw)
                || !formData.TryGetValue("format", out StringValues formatRaw)
                || !formData.TryGetValue("maxDataPoints", out StringValues maxPointsRaw))
                return StatusCode((int)HttpStatusCode.OK);

            var from = DateTimeOffset.FromUnixTimeSeconds(int.Parse(fromRaw.FirstOrDefault())).UtcDateTime;
            var until = DateTimeOffset.FromUnixTimeSeconds(int.Parse(untilRaw.FirstOrDefault())).UtcDateTime;
            var maxDataPoints = int.Parse(maxPointsRaw.FirstOrDefault());
            if (!(formatRaw.FirstOrDefault()?.Equals("json", StringComparison.InvariantCultureIgnoreCase) ?? false))
                return StatusCode((int)HttpStatusCode.BadRequest);

            List<dynamic[]> GetDataPoints(string target)
            {
                var splitted = target.Split('.');
                if (splitted.Length < 2 || !int.TryParse(splitted[1], out int index) || index < 0
                    || !graphViewModels.ContainsKey(splitted[0]) || index >= graphViewModels[splitted[0]].GraphCount())
                    return new List<dynamic[]>();
                var viewModel = graphViewModels[splitted[0]];
                return viewModel.Graph(index).TimestampedPoints().ToList();
            }

            return Json(targetRaw.Select(t => new
            {
                target = t,
                datapoints = GetDataPoints(t)
            }));
        }

        // GET: api/graphite/metrics/find
        // Grafana calls it like this: metrics/find?from=1589353272&query=*&until=1589526192
        [HttpGet("metrics/find")]
        [SuppressMessage("Style", "IDE0060", Justification = "'from' and 'until' are passed in by grafana but not used to exclude graphs.")]
        public ActionResult FindMetrics(int from, int until, string query)
        {
            query = string.IsNullOrWhiteSpace(query) || !query.EndsWith('*') ? string.Empty : query[0..^1];

            return Json(GraphIds.Where(g => g.StartsWith(query, StringComparison.InvariantCultureIgnoreCase))
                .Select(g => g.Substring(query.Length))
                .Select(g => new { expandable = g.Contains('.'), text = g.Split('.').FirstOrDefault() }).Distinct());
        }

        // GET: api/graphite/tags/autoComplete/tags
        [HttpGet("tags/autoComplete/tags")]
        public ActionResult GetTags()
        {
            return Json(new string[] { });
        }
    }
}