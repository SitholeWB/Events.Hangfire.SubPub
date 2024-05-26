namespace SubPub.Hangfire
{
    public interface IHangfireEventHandlerContainer
    {
        public void Publish<TEvent>(TEvent obj, HangfireJobOptions? options = default) where TEvent : class;
    }
}