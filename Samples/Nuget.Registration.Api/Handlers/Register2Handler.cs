using SubPub.Hangfire;
using Nuget.Registration.Api.Events;

namespace Nuget.Registration.Api.Handlers
{
    public class Register2Handler : IHangfireEventHandler<RegisterEvent>
    {
        public Task RunAsync(RegisterEvent obj)
        {
            Console.WriteLine("**************START 2 *********");
            Console.WriteLine($"{obj.Email} has registered");
            Console.WriteLine("**************END 2 *********");

            return Task.CompletedTask;
        }
    }
}