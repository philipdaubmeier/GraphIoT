using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Core
{
    public interface IScopedPollingService
    {
        Task PollValues();
    }
}