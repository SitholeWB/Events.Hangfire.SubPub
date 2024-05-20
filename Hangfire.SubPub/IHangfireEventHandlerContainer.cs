namespace Hangfire.SubPub
{
    public interface IHangfireEventHandlerContainer
    {
        public void Publish<T>(T obj) where T : class;
    }
}