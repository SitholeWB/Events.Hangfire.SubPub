namespace SubPub.Hangfire
{
    public interface IHangfireEventHandlerContainer
    {
        public void Publish<T>(T obj, HangfireJobOptions? options = default) where T : class;
    }
}