using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Hangfire.SubPub
{
    public class HangfireEventHandlerContainer : IHangfireEventHandlerContainer
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly Dictionary<Type, List<Type>> _mappings = new Dictionary<Type, List<Type>>();

        public HangfireEventHandlerContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Subscribe<T, THandler>()
            where T : class
            where THandler : IHangfireEventHandler<T>
        {
            var name = typeof(T);

            if (!_mappings.ContainsKey(name))
            {
                _mappings.Add(name, new List<Type> { });
            }

            _mappings[name].Add(typeof(THandler));
        }

        public void Publish<T>(T obj) where T : class
        {
            var name = typeof(T);

            if (_mappings.ContainsKey(name))
            {
                foreach (var handler in _mappings[name])
                {
                    var service = (IHangfireEventHandler<T>)_serviceProvider.GetRequiredService(handler);
                    BackgroundJob.Enqueue(() => service.RunAsync(obj));
                }
            }
        }
    }
}