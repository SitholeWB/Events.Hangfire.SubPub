using SubPub.Hangfire;
using Nuget.Registration.Api.Events;

namespace Nuget.Registration.Api.Handlers
{
    public class DuplicateRegisterHandler : IHangfireEventHandler<DuplicateRegisterEvent>
    {
        public Task RunAsync(DuplicateRegisterEvent obj)
        {
            Console.WriteLine("**************START 1*********");
            Console.WriteLine($"{obj.Email} has aleady registered");
            Console.WriteLine("**************END 1*********");

            return Task.CompletedTask;
        }
    }
}