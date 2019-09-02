using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel
{
    public abstract class GraphCollectionViewModelMultiResolutionBase<Tentity> : GraphCollectionViewModelDeferredLoadBase<Tentity> where Tentity : class, ITimeSeriesDbEntity
    {
        protected Dictionary<Resolution, DbSet<Tentity>> _dataTables = null;

        protected enum Resolution
        {
            InitialNone,
            LowRes,
            MidRes,
            HighRes
        }
        protected Resolution DataResolution => IsInitialSpan ? Resolution.InitialNone : Span.Duration.TotalMinutes >= 15 ? Resolution.LowRes :
                                                                  Span.Duration.TotalSeconds >= 30 ? Resolution.MidRes : Resolution.HighRes;

        protected GraphCollectionViewModelMultiResolutionBase(Dictionary<Resolution, DbSet<Tentity>> dataTables, Dictionary<string, int> columns)
            : base(dataTables.FirstOrDefault().Value, columns)
        {
            _dataTables = dataTables;
        }

        protected override void InvalidateData()
        {
            _dataTable = SelectDataForResolution(DataResolution);

            base.InvalidateData();

            if (DataResolution == Resolution.LowRes)
            {
                DateTime FirstOfMonth(DateTime date) => date.AddDays(-1 * (date.Day - 1));
                var beginMonth = FirstOfMonth(Span.Begin.Date);
                var endMonth = FirstOfMonth(Span.End.Date);

                data = _dataTable?.Where(x => x.Key >= beginMonth && x.Key <= endMonth);
            }
        }

        private DbSet<Tentity> SelectDataForResolution(Resolution requestedResolution)
        {
            // if the requested resolution is available, use it. If not, use the next higher resolution.
            // If there is no higher resolution use the next lower.
            var resolutions = Enum.GetValues(typeof(Resolution)).Cast<Resolution>();
            var searchSpace = Enumerable.Repeat(requestedResolution, 1)
                .Union(resolutions.Where(x => (int)x > (int)requestedResolution).OrderBy(x => (int)x))
                .Union(resolutions.Where(x => (int)x < (int)requestedResolution).OrderByDescending(x => (int)x));

            return searchSpace.Where(x => _dataTables?.ContainsKey(x) ?? false).Select(x => _dataTables[x]).FirstOrDefault();
        }
    }
}