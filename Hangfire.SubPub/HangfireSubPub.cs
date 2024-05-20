using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hangfire.SubPub
{
    public class HangfireSubPub<TEvent> where TEvent : class
    {
        public Type EventName { get; set; }
        public List<Type> List { get; set; } = new List<Type>();
        private readonly IServiceCollection _services;
        private readonly ServiceLifetime _lifetime;

        public HangfireSubPub(IServiceCollection services, Type eventName, ServiceLifetime lifetime)
        {
            _services = services;
            EventName = eventName;
            _lifetime = lifetime;
        }

        public HangfireSubPub<TEvent> Subscribe<THandler>() where THandler : IHangfireEventHandler<TEvent>
        {
            if (!_services.Any(x => x.ServiceType == typeof(THandler)))
            {
                switch (_lifetime)
                {
                    case ServiceLifetime.Singleton:
                        _services.TryAddSingleton(typeof(THandler));
                        break;

                    case ServiceLifetime.Transient:
                        _services.TryAddTransient(typeof(THandler));
                        break;

                    default:
                        _services.TryAddScoped(typeof(THandler));
                        break;
                }
            }
            var sp = _services.BuildServiceProvider();
            var eventHandlerContainer = sp.GetRequiredService<HangfireEventHandlerContainer>();
            eventHandlerContainer.Subscribe<TEvent, THandler>();
            return this;
        }
    }
}