using PhilipDaubmeier.TimeseriesHostCommon;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.NetatmoHost.Polling
{
    public interface INetatmoPollingService : IScopedPollingService
    {
        Task PollSensorValues(DateTime start, DateTime end);
    }
}