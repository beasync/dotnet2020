using Dapr.Actors;
using System.Threading.Tasks;

namespace AspNetActors.Interfaces
{
    public interface IAccountingActor : IActor
    {
        Task Deposit(float amount);
        Task<float> GetBalanceAsync();
    }
}
