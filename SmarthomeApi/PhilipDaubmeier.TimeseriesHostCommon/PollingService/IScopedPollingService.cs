using System.Threading.Tasks;

namespace PhilipDaubmeier.TimeseriesHostCommon
{
    public interface IScopedPollingService
    {
        Task PollValues();
    }
}