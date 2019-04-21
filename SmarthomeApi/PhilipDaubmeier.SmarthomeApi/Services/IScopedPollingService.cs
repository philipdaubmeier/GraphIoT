using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Services
{
    public interface IScopedPollingService
    {
        Task PollValues();
    }
}