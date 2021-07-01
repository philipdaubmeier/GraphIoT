using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Aggregation;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Graphite.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class GraphDataSource
    {
        private readonly List<IGraphCollectionViewModel> _viewModels;

        public GraphDataSource(IEnumerable<IGraphCollectionViewModel> viewModels) => _viewModels = viewModels.ToList();

        public TimeSeriesSpan Span { set { foreach (var model in _viewModels) model.Span = value; } get { return _viewModels.FirstOrDefault().Span; } }

        public Aggregator AggregatorFunction { set { foreach (var model in _viewModels) model.AggregatorFunction = value; } }

        public double CorrectionFactor { set { foreach (var model in _viewModels) model.CorrectionFactor = value; } }

        public double CorrectionOffset { set { foreach (var model in _viewModels) model.CorrectionOffset = value; } }

        private List<string>? _graphKeys = null;
        public List<string> GraphKeys
        {
            get
            {
                if (_graphKeys is null)
                {
                    Regex nonAlphaPunctuation = new("[^A-Za-z_.]+");
                    string normalizeGraphKey(string key)
                    {
                        return nonAlphaPunctuation.Replace(key.Replace('_', '.').Replace(' ', '_'), string.Empty);
                    };

                    TimeSeriesSpan prevSpanToRestore = Span;
                    Span = new(DateTime.MinValue, DateTime.MinValue.AddMinutes(1), 1);

                    _graphKeys = _viewModels.SelectMany(collection => collection.GraphKeys()
                        .Select(graphKey => normalizeGraphKey($"{collection.Key}.{graphKey}"))).ToList();

                    Span = prevSpanToRestore;
                }

                return _graphKeys;
            }
        }

        public IEnumerable<GraphViewModel> Query(string query)
        {
            var q = new TargetQuery(query);
            foreach (var key in GraphKeys.Where(g => q.IsMatch(g)))
            {
                GraphViewModel? graph = null;
                try
                {
                    var splitted = key.Split('.');
                    if (splitted.Length < 1)
                        continue;
                    var viewModelKey = splitted[0];

                    var viewModelIndex = GraphKeys.FindIndex(k => k.StartsWith(viewModelKey + "."));
                    var graphIndex = GraphKeys.FindIndex(k => k == key);
                    var index = graphIndex - viewModelIndex;

                    var viewModel = _viewModels.Find(x => x.Key == viewModelKey);
                    if (viewModel is null || index < 0 || index >= viewModel.GraphCount())
                        continue;

                    graph = viewModel.Graph(index);
                }
                catch { continue; }

                if (graph != null)
                    yield return graph;
            }
            yield break;
        }

        /// <summary>
        /// Traverses the tree of available keys by the given query string with wildcards
        /// and outputs the list of distinct next path segments, including a hint if this
        /// is a tree leaf or is further expandable.
        ///
        /// Example GraphKeys list:
        /// [
        ///     "root.subitem1.leaf11",
        ///     "root.subitem2.leaf21",
        ///     "root.subitem2.leaf22",
        ///     "root.subitem2.subsubitem21.leaf211",
        ///     "other.subitem2.otherleaf21"
        /// ]
        ///
        /// then AutocompletePath("root.*") would return
        /// [
        ///     (true, "subitem1"),
        ///     (true, "subitem2")
        /// ]
        ///
        /// and AutocompletePath("*.subitem2.*") would return
        /// [
        ///     (false, "leaf21"),
        ///     (false, "leaf22"),
        ///     (true, "subsubitem21"),
        ///     (false, "otherleaf21")
        /// ]
        /// </summary>
        public IEnumerable<(bool expandable, string segment)> AutocompletePath(string query)
        {
            var q = new TargetQuery(query);
            return GraphKeys.Where(g => q.IsMatch(g)).Select(g => q.NextSegment(g)).Distinct();
        }
    }
}