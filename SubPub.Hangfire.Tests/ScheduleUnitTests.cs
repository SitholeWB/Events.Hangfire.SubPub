using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SubPub.Hangfire.Tests.Events;
using SubPub.Hangfire.Tests.Handlers;

namespace SubPub.Hangfire.Tests
{
    public partial class UnitTests : BaseUnitTests
    {
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

        [Fact]
        public void Schedulee_RunAsync_OneEventAndTwoHandlers_ShouldCallRunAsyncTwice()
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
            },
            new HangfireJobOptions
            {
                HangfireJobType = HangfireJobType.Schedule,
                TimeSpan = TimeSpan.FromSeconds(15),
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Exactly(2));
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Once());
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(Test2Handler) && job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Once());
        }

        [Fact]
        public void Schedule_RunAsync_TwoEventsAndThreeHandlers_ShouldCallRunAsync3Times()
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
            },
            new HangfireJobOptions
            {
                HangfireJobType = HangfireJobType.Schedule,
                TimeSpan = TimeSpan.FromSeconds(15),
            });
            _hangfireEventHandlerContainer.Publish(new AnotherEvent
            {
                Name = "Bob"
            },
            new HangfireJobOptions
            {
                HangfireJobType = HangfireJobType.Schedule,
                TimeSpan = TimeSpan.FromSeconds(15),
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Exactly(3));
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Once());
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(Test2Handler) && job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Once());
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(AnotherHandler) && job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Once());
        }

        [Fact]
        public void Schedule_RunAsync_TwoEventsAndThreeHandlers_BUT_PublishToOneHandler_ShouldCallRunAsyncOnce()
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
            },
            new HangfireJobOptions
            {
                HangfireJobType = HangfireJobType.Schedule,
                TimeSpan = TimeSpan.FromSeconds(15),
            });

            // Assert
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Exactly(1));
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(TestHandler) && job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Never());
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(Test2Handler) && job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Never());
            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Type == typeof(AnotherHandler) && job.Method.Name == "RunAsync"), It.IsAny<ScheduledState>()), Times.Once());
        }
    }
}