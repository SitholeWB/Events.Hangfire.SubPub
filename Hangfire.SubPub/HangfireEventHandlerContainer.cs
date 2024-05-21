using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Hangfire.SubPub
{
    public class HangfireEventHandlerContainer : IHangfireEventHandlerContainer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBackgroundJobClient _jobClient;
        private static readonly Dictionary<Type, HashSet<Type>> _mappings = new Dictionary<Type, HashSet<Type>>();

        public HangfireEventHandlerContainer(IServiceProvider serviceProvider, IBackgroundJobClient jobClient)
        {
            _serviceProvider = serviceProvider;
            _jobClient = jobClient;
        }

        public void Subscribe<TEvent, THandler>(params Type[] events)
            where TEvent : class
            where THandler : IHangfireEventHandler<TEvent>
        {
            var name = typeof(TEvent);

            if (!_mappings.ContainsKey(name))
            {
                _mappings.Add(name, new HashSet<Type> { });
            }

            _mappings[name].Add(typeof(THandler));
            foreach (var eventType in events)
            {
                if (!_mappings.ContainsKey(eventType))
                {
                    _mappings.Add(eventType, new HashSet<Type> { });
                }
                _mappings[eventType].Add(typeof(THandler));
            }
        }

        public void Publish<TEvent>(TEvent obj, HangfireJobOptions? options = default) where TEvent : class
        {
            var name = typeof(TEvent);

            if (_mappings.ContainsKey(name))
            {
                if (options?.HangfireJobType == HangfireJobType.Schedule && options.TimeSpan != TimeSpan.Zero)
                {
                    foreach (var handler in _mappings[name])
                    {
                        var service = (IHangfireEventHandler<TEvent>)_serviceProvider.GetRequiredService(handler);
                        _jobClient.Schedule(() => service.RunAsync(obj), options.TimeSpan);
                    }
                }
                else
                {
                    foreach (var handler in _mappings[name])
                    {
                        var service = (IHangfireEventHandler<TEvent>)_serviceProvider.GetRequiredService(handler);
                        _jobClient.Enqueue(() => service.RunAsync(obj));
                    }
                }
            }
        }
    }
}