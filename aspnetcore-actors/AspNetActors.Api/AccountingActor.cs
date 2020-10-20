using AspNetActors.Interfaces;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using System;
using System.Threading.Tasks;

namespace AspNetActors.Api
{
    internal class AccountingActor : Actor, IAccountingActor
    {
        public AccountingActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        protected override async Task OnActivateAsync()
        {
            // Provides opportunity to perform some optional setup.
            Console.WriteLine($"Activating actor id: {this.Id}");

            await this.StateManager.TryRemoveStateAsync("balance");
            await this.StateManager.AddStateAsync<float>("balance", 0);
        }

        protected override Task OnDeactivateAsync()
        {
            // Provides Opporunity to perform optional cleanup.
            Console.WriteLine($"Deactivating actor id: {this.Id}");
            return Task.CompletedTask;
        }

        public async Task Deposit(float amount)
        {
            // Data is saved to configured state store implicitly after each method execution by Actor's runtime.
            // Data can also be saved explicitly by calling this.StateManager.SaveStateAsync();
            // State to be saved must be DataContract serializable.
            var balance = await this.StateManager.GetOrAddStateAsync<float>("balance", 0);
            await this.StateManager.SetStateAsync<float>(
                "balance",          // state name
                balance + amount);  // data saved for the named state "balance"
        }

        public Task<float> GetBalanceAsync()
        {
            // Gets state from the state store.
            return this.StateManager.GetStateAsync<float>("balance");
        }
    }
}
