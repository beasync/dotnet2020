using AspNetActors.Interfaces;
using Dapr.Actors;
using Dapr.Actors.Client;
using System;
using System.Threading.Tasks;

namespace AspNetActors.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var actorType = "AccountingActor";  // Registered Actor Type in Actor Service
            var actorID = new ActorId("1");

            // Create the local proxy by using the same interface that the service implements
            // By using this proxy, you can call strongly typed methods on the interface using Remoting.
            var proxy = ActorProxy.Create<IAccountingActor>(actorID, actorType);

            await proxy.Deposit(5);
            await proxy.Deposit(6);

            var balance = await proxy.GetBalanceAsync();
            Console.WriteLine(balance);
        }
    }
}
