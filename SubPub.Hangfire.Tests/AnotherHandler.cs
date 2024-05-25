namespace SubPub.Hangfire.Tests
{
    public class AnotherHandler : IHangfireEventHandler<AnotherEvent>
    {
        public Task RunAsync(AnotherEvent obj)
        {
            return Task.CompletedTask;
        }
    }
}