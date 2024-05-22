using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;

namespace SubPub.Hangfire
{
    public static class HangfireExtensions
    {
        public static HangfireSubPub<T> AddHangfireSubPub<T>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where T : class
        {
            var hangfireSubPub = new HangfireSubPub<T>(services, typeof(T), serviceLifetime);
            if (!services.Any(x => x.ServiceType == typeof(HangfireEventHandlerContainer)))
            {
                switch (serviceLifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.TryAddSingleton<HangfireEventHandlerContainer>();
                        services.TryAddSingleton<IHangfireEventHandlerContainer, HangfireEventHandlerContainer>();
                        break;

                    case ServiceLifetime.Transient:
                        services.TryAddTransient<HangfireEventHandlerContainer>();
                        services.TryAddTransient<IHangfireEventHandlerContainer, HangfireEventHandlerContainer>();
                        break;

                    default:
                        services.TryAddScoped<HangfireEventHandlerContainer>();
                        services.TryAddScoped<IHangfireEventHandlerContainer, HangfireEventHandlerContainer>();
                        break;
                }
            }
            return hangfireSubPub;
        }
    }
}