using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Reflection;

namespace SubPub.Hangfire.Tests
{
    public class BaseUnitTests
    {
        protected readonly Mock<IBackgroundJobClient> _backgroundJobClient;
        protected readonly Mock<IServiceProvider> _serviceProvider;
        protected readonly IServiceCollection _services;

        public BaseUnitTests()
        {
            _backgroundJobClient = new Mock<IBackgroundJobClient>();
            _serviceProvider = new Mock<IServiceProvider>();
            _services = new ServiceCollection();
            var _hangfireEventHandlerContainer = new HangfireEventHandlerContainer(_serviceProvider.Object, _backgroundJobClient.Object);

            _services.TryAddScoped<HangfireEventHandlerContainer>(x => _hangfireEventHandlerContainer);
            _services.TryAddScoped<IHangfireEventHandlerContainer>(x => _hangfireEventHandlerContainer);

            var field = typeof(HangfireEventHandlerContainer).GetField("_eventHandlers", BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(null, new Dictionary<Type, HashSet<Type>>());
            }
        }
    }
}