using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.App.Database;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Structure;
using PhilipDaubmeier.GraphIoT.Digitalstrom.ViewModel;
using PhilipDaubmeier.GraphIoT.Sonnen.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PhilipDaubmeier.GraphIoT.App.Controllers
{
    [Route("api/lametric")]
    public class LaMetricController : Controller
    {
        private readonly PersistenceContext _dbContext;
        private readonly IDigitalstromStructureService _dsStructure;

        public LaMetricController(PersistenceContext databaseContext, IDigitalstromStructureService dsStructure)
        {
            _dbContext = databaseContext;
            _dsStructure = dsStructure;
        }

        // GET: api/lametric/powerconsumption
        [HttpGet("powerconsumption")]
        public ActionResult GetLaMetricPowerconsumption()
        {
            var viewModel = new DigitalstromEnergyViewModel(_dbContext, _dsStructure)
            {
                Span = new TimeSeriesSpan(DateTime.Now.AddMinutes(-37), DateTime.Now, TimeSeriesSpan.Spacing.Spacing1Min)
            };
            var singlePowerValues = viewModel.Graphs().Select(x => x.Points.ToList()).ToList();
            var chartPowerValues = Enumerable.Range(0, Math.Min(37, singlePowerValues.Min(x => x.Count)))
                .Select(i => singlePowerValues.Sum(x => (int)(double)x[i])).ToList();

            return Json(new
            {
                frames = new List<object>() {
                    new
                    {
                        text = $"{chartPowerValues.LastOrDefault()} W",
                        icon = "a21256"
                    },
                    new
                    {
                        index = 1,
                        chartData = chartPowerValues
                    }
                }
            });
        }

        // GET: api/lametric/sonnen
        [HttpGet("sonnen")]
        public ActionResult GetLaMetricSonnen()
        {
            var viewModel = new SonnenEnergyViewModel(_dbContext)
            {
                Span = new TimeSeriesSpan(DateTime.Now.Date, DateTime.Now, TimeSeriesSpan.Spacing.Spacing1Min)
            };
            var totalYieldToday = viewModel.Graph(0).Points.Select(x => (decimal?)x).Where(x => x.HasValue).Sum(x => x.Value) / 60 / 1000;

            viewModel.Span = new TimeSeriesSpan(DateTime.Now.Date, DateTime.Now.Date.AddDays(1), 37);
            var chartSolarYield = viewModel.Graph(0).Points;

            return Json(new
            {
                frames = new List<object>() {
                    new
                    {
                        text = $"{totalYieldToday:0.#} kWh",
                        icon = "a27283"
                    },
                    new
                    {
                        index = 2,
                        chartData = chartSolarYield
                    }
                }
            });
        }

        // GET: api/lametric/viessmann/solar
        [HttpGet("viessmann/solar")]
        public ActionResult GetLaMetricViessmannSolar()
        {
            var dbSolarSeries = _dbContext.ViessmannSolarTimeseries.Where(x => x.Key == DateTime.Now.Date).FirstOrDefault();
            if (dbSolarSeries == null)
                return StatusCode((int)HttpStatusCode.NotFound);

            var totalYieldToday = (double)dbSolarSeries.SolarWhTotal / 1000d;
            var chartSolarYield = dbSolarSeries.SolarWhSeries.Trimmed(0).TakeLast(37);
            var currentCollectorTemp = dbSolarSeries.SolarCollectorTempSeries.Reverse()
                .SkipWhile(x => !x.Value.HasValue).FirstOrDefault().Value;

            return Json(new
            {
                frames = new List<object>() {
                    new
                    {
                        text = $"{totalYieldToday:0.#} kWh",
                        icon = "a30793"
                    },
                    new
                    {
                        index = 2,
                        chartData = chartSolarYield
                    },
                    new
                    {
                        text = $"{currentCollectorTemp:0.#} °C",
                        icon = "a27285"
                    }
                }
            });
        }
    }
}