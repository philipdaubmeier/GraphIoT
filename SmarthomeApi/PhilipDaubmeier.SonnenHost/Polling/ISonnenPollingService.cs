using PhilipDaubmeier.TimeseriesHostCommon;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SonnenHost.Polling
{
    public interface ISonnenPollingService : IScopedPollingService
    {
        Task PollSensorValues(DateTime start, DateTime end);
    }
}