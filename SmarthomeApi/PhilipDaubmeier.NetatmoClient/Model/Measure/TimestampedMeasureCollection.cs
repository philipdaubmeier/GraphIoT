using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class TimestampedMeasureCollection
    {
        public int BegTime { get; set; }
        public int StepTime { get; set; }
        public List<List<double?>> Value { get; set; }
    }
}