namespace SubPub.Hangfire.Tests
{
    public class TestHandler : IHangfireEventHandler<TestEvent>
    {
        public Task RunAsync(TestEvent obj)
        {
            return Task.CompletedTask;
        }
    }
}