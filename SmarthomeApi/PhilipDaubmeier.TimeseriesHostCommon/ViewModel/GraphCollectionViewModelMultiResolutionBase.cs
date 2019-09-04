using PhilipDaubmeier.TimeseriesHostCommon.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel
{
    public abstract class GraphCollectionViewModelMultiResolutionBase<Tentity> : GraphCollectionViewModelDeferredLoadBase<Tentity> where Tentity : class, ITimeSeriesDbEntity
    {
        protected Dictionary<Resolution, IQueryable<Tentity>> _dataTables = null;
        protected Dictionary<Resolution, TimeSpan> _spans = null;

        protected enum Resolution
        {
            InitialNone,
            LowRes,
            MidRes,
            HighRes
        }

        private double LowResSeconds => _spans.ContainsKey(Resolution.LowRes) ? _spans[Resolution.LowRes].TotalSeconds / 2d : 15d * 60d;
        private double MidResSeconds => _spans.ContainsKey(Resolution.MidRes) ? _spans[Resolution.MidRes].TotalSeconds / 2d : 60d;

        protected Resolution DataResolution => IsInitialSpan ? Resolution.InitialNone :
                 Span.Duration.TotalSeconds >= LowResSeconds ? Resolution.LowRes :
                 Span.Duration.TotalSeconds >= MidResSeconds ? Resolution.MidRes : Resolution.HighRes;

        protected GraphCollectionViewModelMultiResolutionBase(Dictionary<Resolution, IQueryable<Tentity>> dataTables, Dictionary<string, int> columns)
            : base(dataTables.FirstOrDefault().Value, columns)
        {
            _dataTables = dataTables;
            _spans = dataTables.Select(t => new Tuple<Resolution, Type>(t.Key, t.Value.GetType().GenericTypeArguments.FirstOrDefault()))
                               .ToDictionary(t => t.Item1, t => (Activator.CreateInstance(t.Item2) as TimeSeriesDbEntityBase).Span.Duration);
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

        private IQueryable<Tentity> SelectDataForResolution(Resolution requestedResolution)
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