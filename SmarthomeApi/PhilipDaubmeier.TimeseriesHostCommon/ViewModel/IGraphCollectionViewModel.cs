using System.Collections.Generic;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel
{
    public interface IGraphCollectionViewModel
    {
        int GraphCount();

        GraphViewModel Graph(int index);

        IEnumerable<GraphViewModel> Graphs();
    }
}