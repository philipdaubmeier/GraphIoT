using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.SmarthomeApi.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.Linq;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Route("api/conversion")]
    public class ConversionController : Controller
    {
        private readonly PersistenceContext db;

        public ConversionController(PersistenceContext databaseContext)
        {
            db = databaseContext;
        }

        // DELETE api/conversion/allseries/computedlowres
        [HttpDelete("allseries/computedlowres")]
        public ActionResult DeleteComputedLowResolutions()
        {
            void Clear<Tdb>(DbSet<Tdb> dbSet) where Tdb : TimeSeriesDbEntityBase
            {
                dbSet.RemoveRange(dbSet);
                db.SaveChanges();
            }

            Clear(db.DsEnergyMidresDataSet);
            Clear(db.DsEnergyLowresDataSet);
            Clear(db.DsSensorLowresDataSet);
            Clear(db.SonnenEnergyDataSet);
            Clear(db.SonnenEnergyLowresDataSet);
            Clear(db.ViessmannHeatingLowresTimeseries);
            Clear(db.ViessmannSolarLowresTimeseries);

            return StatusCode(200);
        }

        // POST api/conversion/allseries/convert2utc
        [HttpPost("allseries/convert2utc")]
        public ActionResult ConvertSensor2Utc([FromQuery] string begin, [FromQuery] string end)
        {
            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode(404);

            DateTime FromLocalToUtc(DateTime dt) => new DateTime(dt.Ticks, DateTimeKind.Local).ToUniversalTime();

            void ConvertOneTimeSeriesOneAndPrevDay<Tdb, Tval>(DateTime day, DbSet<Tdb> dbSet, Func<Tdb, Tdb, bool> dbComparer, int index, Func<Tdb, TimeSeries<Tval>> seriesSelector) where Tdb : TimeSeriesDbEntityBase where Tval : struct
            {
                var dateCurr = day;
                var datePrev = dateCurr.AddDays(-1);
                var loaded = dbSet.Where(x => x.Key == dateCurr || x.Key == datePrev).ToList();
                foreach (var pair in loaded.Where(x => x.Key == dateCurr).Select(x => new Tuple<Tdb, Tdb>(x, loaded.Where(y => y.Key == datePrev && dbComparer(x, y)).FirstOrDefault())))
                {
                    var orig = seriesSelector(pair.Item1);
                    var origPrev = pair.Item2 == null ? null : seriesSelector(pair.Item2);
                    DateTime? lastTime = null;
                    foreach (var item in orig.Select(x => x.Key).ToList())
                    {
                        lastTime = FromLocalToUtc(item);
                        orig[lastTime.Value] = orig[item];
                        if (origPrev != null)
                            origPrev[lastTime.Value] = orig[item];
                    }

                    if (lastTime.HasValue)
                    {
                        foreach (var time in Enumerable.Range(1, (int)(60 * 60 * 2 / orig.Span.Duration.TotalSeconds)).Select(i => lastTime.Value + i * orig.Span.Duration))
                            orig[time] = null;
                    }

                    pair.Item1.SetSeries(index, orig);
                    pair.Item2?.SetSeries(index, origPrev);
                }
            }

            foreach (var day in span.IncludedDates().Select(x => x.ToUniversalTime()))
            {
                ConvertOneTimeSeriesOneAndPrevDay(day, db.DsSensorDataSet, (db1, db2) => db1.ZoneId == db2.ZoneId, 0, db => db.TemperatureSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.DsSensorDataSet, (db1, db2) => db1.ZoneId == db2.ZoneId, 1, db => db.HumiditySeries);

                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannSolarTimeseries, (db1, db2) => true, 0, db => db.SolarWhSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannSolarTimeseries, (db1, db2) => true, 1, db => db.SolarCollectorTempSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannSolarTimeseries, (db1, db2) => true, 2, db => db.SolarHotwaterTempSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannSolarTimeseries, (db1, db2) => true, 3, db => db.SolarPumpStateSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannSolarTimeseries, (db1, db2) => true, 4, db => db.SolarSuppressionSeries);

                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 0, db => db.BurnerMinutesSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 1, db => db.BurnerStartsSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 2, db => db.BurnerModulationSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 3, db => db.OutsideTempSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 4, db => db.BoilerTempSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 5, db => db.BoilerTempMainSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 6, db => db.Circuit0TempSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 7, db => db.Circuit1TempSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 8, db => db.DhwTempSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 9, db => db.BurnerActiveSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 10, db => db.Circuit0PumpSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 11, db => db.Circuit1PumpSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 12, db => db.DhwPrimaryPumpSeries);
                ConvertOneTimeSeriesOneAndPrevDay(day, db.ViessmannHeatingTimeseries, (db1, db2) => true, 13, db => db.DhwCirculationPumpSeries);

                db.SaveChanges();
            }

            return StatusCode(200);
        }
    }
}