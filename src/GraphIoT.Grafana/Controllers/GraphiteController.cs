using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;

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
        public ActionResult Render()
        {
            return StatusCode((int)HttpStatusCode.OK);
        }

        // GET: api/graphite/metrics/find
        // Grafana calls it like this: metrics/find?from=1589353272&query=*&until=1589526192
        [HttpGet("metrics/find")]
        [SuppressMessage("Style", "IDE0060", Justification = "'from' and 'until' are passed in by grafana but not used to exclude graphs.")]
        public ActionResult FindMetrics(int from, int until, string query)
        {
            query = string.IsNullOrWhiteSpace(query) || !query.EndsWith('*') ? string.Empty : query.Substring(0, query.Length - 1);

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