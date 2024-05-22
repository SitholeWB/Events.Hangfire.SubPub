using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Events.Hangfire.SubPub
{
    public class HangfireEventHandlerContainer : IHangfireEventHandlerContainer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBackgroundJobClient _jobClient;
        private static readonly IDictionary<Type, HashSet<Type>> _eventHandlers = new Dictionary<Type, HashSet<Type>>();

        public HangfireEventHandlerContainer(IServiceProvider serviceProvider, IBackgroundJobClient jobClient)
        {
            _serviceProvider = serviceProvider;
            _jobClient = jobClient;
        }

        public void Subscribe<TEvent, THandler>()
            where TEvent : class
            where THandler : IHangfireEventHandler<TEvent>
        {
            var name = typeof(TEvent);

            if (!_eventHandlers.ContainsKey(name))
            {
                _eventHandlers.Add(name, new HashSet<Type> { });
            }
            _eventHandlers[name].Add(typeof(THandler));
        }

        public void Publish<TEvent>(TEvent obj, HangfireJobOptions? options = default) where TEvent : class
        {
            var name = typeof(TEvent);

            if (_eventHandlers.ContainsKey(name))
            {
                if (options?.HangfireJobType == HangfireJobType.Schedule && options.TimeSpan != TimeSpan.Zero)
                {
                    foreach (var handler in _eventHandlers[name])
                    {
                        var service = (IHangfireEventHandler<TEvent>)_serviceProvider.GetRequiredService(handler);
                        _jobClient.Schedule(() => service.RunAsync(obj), options.TimeSpan);
                    }
                }
                else
                {
                    foreach (var handler in _eventHandlers[name])
                    {
                        var service = (IHangfireEventHandler<TEvent>)_serviceProvider.GetRequiredService(handler);
                        _jobClient.Enqueue(() => service.RunAsync(obj));
                    }
                }
            }
        }
    }
}