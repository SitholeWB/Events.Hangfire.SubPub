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
    public class EnqueueUnitTests
    {
        private readonly Mock<IBackgroundJobClient> _backgroundJobClient;
        private readonly Mock<IServiceProvider> _serviceProvider;
        private readonly IServiceCollection _services;

        public EnqueueUnitTests()
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
        public void Enqueue_RunAsync_ZeroEventAndZeroHandler_ShouldNotCallRunAsync()
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
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Never);
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Never);
        }

        [Fact]
        public void Enqueue_RunAsync_Enqueue_OneEventAndOneHandler_ShouldNotSchedule()
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
                HangfireJobType = HangfireJobType.Enqueue,
                TimeSpan = TimeSpan.FromSeconds(15),
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Never);
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Never);
        }

        [Fact]
        public void Enqueue_RunAsync_EventWithNoHandler_ShouldNotCallRunAsync()
        {
            // Arrange
            _services.AddHangfireSubPub<TestEvent>()
                     .Subscribe<TestHandler>();
            _serviceProvider.Setup(s => s.GetService(typeof(TestHandler)))
                     .Returns(new TestHandler());

            var provider = _services.BuildServiceProvider();
            var _hangfireEventHandlerContainer = provider.GetRequiredService<IHangfireEventHandlerContainer>();

            // Act
            _hangfireEventHandlerContainer.Publish(new AnotherEvent
            {
                Name = "Bob"
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Never);
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Never);
        }

        [Fact]
        public void Enqueue_RunAsync_OneEventAndOneHandler_ShouldCallRunAsyncOnce()
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
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()));
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Enqueue_RunAsync_OneEventAndTwoHandlers_ShouldCallRunAsyncTwice()
        {
            // Arrange
            _services.AddHangfireSubPub<TestEvent>()
                     .Subscribe<TestHandler>()
                     .Subscribe<Test2Handler>();
            _serviceProvider.Setup(s => s.GetService(typeof(TestHandler)))
                     .Returns(new TestHandler());
            _serviceProvider.Setup(s => s.GetService(typeof(Test2Handler)))
                     .Returns(new Test2Handler());

            var provider = _services.BuildServiceProvider();
            var _hangfireEventHandlerContainer = provider.GetRequiredService<IHangfireEventHandlerContainer>();

            // Act
            _hangfireEventHandlerContainer.Publish(new TestEvent
            {
                Name = "Bob"
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Exactly(2));
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Once());
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(Test2Handler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Once());
        }

        [Fact]
        public void Enqueue_RunAsync_TwoEventsAndThreeHandlers_ShouldCallRunAsync3Times()
        {
            // Arrange
            _services.AddHangfireSubPub<TestEvent>()
                     .Subscribe<TestHandler>()
                     .Subscribe<Test2Handler>();
            _services.AddHangfireSubPub<AnotherEvent>()
                     .Subscribe<AnotherHandler>();

            _serviceProvider.Setup(s => s.GetService(typeof(TestHandler)))
                     .Returns(new TestHandler());
            _serviceProvider.Setup(s => s.GetService(typeof(Test2Handler)))
                     .Returns(new Test2Handler());
            _serviceProvider.Setup(s => s.GetService(typeof(AnotherHandler)))
                     .Returns(new AnotherHandler());

            var provider = _services.BuildServiceProvider();
            var _hangfireEventHandlerContainer = provider.GetRequiredService<IHangfireEventHandlerContainer>();

            // Act
            _hangfireEventHandlerContainer.Publish(new TestEvent
            {
                Name = "Bob"
            });
            _hangfireEventHandlerContainer.Publish(new AnotherEvent
            {
                Name = "Bob"
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Exactly(3));
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Once());
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(Test2Handler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Once());
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(AnotherHandler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Once());
        }

        [Fact]
        public void Enqueue_RunAsync_TwoEventsAndThreeHandlers_BUT_PublishToOneHandler_ShouldCallRunAsyncOnce()
        {
            // Arrange
            _services.AddHangfireSubPub<TestEvent>()
                     .Subscribe<TestHandler>()
                     .Subscribe<Test2Handler>();
            _services.AddHangfireSubPub<AnotherEvent>()
                     .Subscribe<AnotherHandler>();

            _serviceProvider.Setup(s => s.GetService(typeof(TestHandler)))
                     .Returns(new TestHandler());
            _serviceProvider.Setup(s => s.GetService(typeof(Test2Handler)))
                     .Returns(new Test2Handler());
            _serviceProvider.Setup(s => s.GetService(typeof(AnotherHandler)))
                     .Returns(new AnotherHandler());

            var provider = _services.BuildServiceProvider();
            var _hangfireEventHandlerContainer = provider.GetRequiredService<IHangfireEventHandlerContainer>();

            // Act
            _hangfireEventHandlerContainer.Publish(new AnotherEvent
            {
                Name = "Bob"
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Exactly(1));
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Never());
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(Test2Handler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Never());
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(AnotherHandler) && job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()), Times.Once());
        }
    }
}