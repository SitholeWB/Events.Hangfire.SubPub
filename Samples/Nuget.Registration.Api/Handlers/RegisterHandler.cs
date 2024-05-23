using SubPub.Hangfire;
using Nuget.Registration.Api.Events;

namespace Nuget.Registration.Api.Handlers
{
    public class RegisterHandler : IHangfireEventHandler<RegisterEvent>
    {
        public Task RunAsync(RegisterEvent obj)
        {
            Console.WriteLine("**************START 1*********");
            Console.WriteLine($"{obj.Email} has registered");
            Console.WriteLine("**************END 1*********");

            return Task.CompletedTask;
        }
    }
}