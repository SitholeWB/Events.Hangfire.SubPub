using SubPub.Hangfire.Tests.Events;

namespace SubPub.Hangfire.Tests.Handlers
{
    public class AnotherHandler : IHangfireEventHandler<AnotherEvent>
    {
        public Task RunAsync(AnotherEvent obj)
        {
            return Task.CompletedTask;
        }
    }
}