using System.Threading.Tasks;

namespace Hangfire.SubPub
{
    public interface IHangfireEventHandler<T> where T : class
    {
        Task RunAsync(T obj);
    }
}