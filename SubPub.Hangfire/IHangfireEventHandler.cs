using System.Threading.Tasks;

namespace SubPub.Hangfire
{
    public interface IHangfireEventHandler<T> where T : class
    {
        Task RunAsync(T obj);
    }
}