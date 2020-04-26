using PhilipDaubmeier.GraphIoT.Core;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Netatmo.Polling
{
    public interface INetatmoPollingService : IScopedPollingService
    {
        Task PollSensorValues(DateTime start, DateTime end);
    }
}