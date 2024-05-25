namespace SubPub.Hangfire.Tests
{
    public class Test2Handler : IHangfireEventHandler<TestEvent>
    {
        public Task RunAsync(TestEvent obj)
        {
            return Task.CompletedTask;
        }
    }
}