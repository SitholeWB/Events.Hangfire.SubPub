using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using SubPub.Hangfire.Tests.Events;
using SubPub.Hangfire.Tests.Handlers;
using System.Reflection;

namespace SubPub.Hangfire.Tests
{
    public class ScheduleUnitTests
    {
        private readonly Mock<IBackgroundJobClient> _backgroundJobClient;
        private readonly Mock<IServiceProvider> _serviceProvider;
        private readonly IServiceCollection _services;

        public ScheduleUnitTests()
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

        [Fact]
        public void Schedule_RunAsync_ZeroEventAndZeroHandler_ShouldNotCallRunAsync()
        {
            // Arrange
            _serviceProvider.Setup(s => s.GetService(typeof(TestHandler)))
                     .Returns(new TestHandler());

            var provider = _services.BuildServiceProvider();
            var _hangfireEventHandlerContainer = provider.GetRequiredService<IHangfireEventHandlerContainer>();

            // Act
            _hangfireEventHandlerContainer.Publish(new TestEvent
            {
                Name = "Bob"
            },
            new HangfireJobOptions
            {
                HangfireJobType = HangfireJobType.Schedule,
                TimeSpan = TimeSpan.FromSeconds(15),
            }
            );

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Never);
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Never);
        }

        [Fact]
        public void Schedule_RunAsync_Schedule_OneEventAndOneHandler_ShouldNotEnqueue()
        {
            // Arrange
            _services.AddHangfireSubPub<TestEvent>()
                     .Subscribe<TestHandler>();
            _serviceProvider.Setup(s => s.GetService(typeof(TestHandler)))
                     .Returns(new TestHandler());

            var provider = _services.BuildServiceProvider();
            var _hangfireEventHandlerContainer = provider.GetRequiredService<IHangfireEventHandlerContainer>();

            // Act
            _hangfireEventHandlerContainer.Publish(new TestEvent
            {
                Name = "Bob"
            },
            new HangfireJobOptions
            {
                HangfireJobType = HangfireJobType.Schedule,
                TimeSpan = TimeSpan.FromSeconds(15),
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Never);
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Never);
        }

        [Fact]
        public void Schedule_RunAsync_OneEventAndOneHandler_ShouldCallRunAsyncOnce()
        {
            // Arrange
            _services.AddHangfireSubPub<TestEvent>()
                     .Subscribe<TestHandler>();
            _serviceProvider.Setup(s => s.GetService(typeof(TestHandler)))
                     .Returns(new TestHandler());

            var provider = _services.BuildServiceProvider();
            var _hangfireEventHandlerContainer = provider.GetRequiredService<IHangfireEventHandlerContainer>();

            // Act
            _hangfireEventHandlerContainer.Publish(new TestEvent
            {
                Name = "Bob"
            },
            new HangfireJobOptions
            {
                HangfireJobType = HangfireJobType.Schedule,
                TimeSpan = TimeSpan.FromSeconds(15),
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()));
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()));
        }
    }
}