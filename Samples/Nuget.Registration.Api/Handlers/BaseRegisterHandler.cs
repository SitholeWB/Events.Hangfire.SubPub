using SubPub.Hangfire;
using Nuget.Registration.Api.Events;

namespace Nuget.Registration.Api.Handlers
{
    public class BaseRegisterHandler : IHangfireEventHandler<BaseRegisterEvent>
    {
        public Task RunAsync(BaseRegisterEvent obj)
        {
            Console.WriteLine("**************START 1*********");
            Console.WriteLine($"PhysicalAddress {obj.PhysicalAddress} has registered");
            Console.WriteLine("**************END 1*********");

            return Task.CompletedTask;
        }
    }
}