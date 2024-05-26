using SubPub.Hangfire.Tests.Events;

namespace SubPub.Hangfire.Tests.Handlers
{
    public class Test2Handler : IHangfireEventHandler<TestEvent>
    {
        public Task RunAsync(TestEvent obj)
        {
            return Task.CompletedTask;
        }
    }
}