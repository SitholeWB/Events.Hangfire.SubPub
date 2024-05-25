using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace SubPub.Hangfire.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var _backgroundJobClient = new Mock<IBackgroundJobClient>();
            var _serviceProvider = new Mock<IServiceProvider>();
            var services = new ServiceCollection();
            var _hangfireEventHandlerContainer = new HangfireEventHandlerContainer(_serviceProvider.Object, _backgroundJobClient.Object);

            services.TryAddScoped<HangfireEventHandlerContainer>(x => _hangfireEventHandlerContainer);
            services.TryAddScoped<IHangfireEventHandlerContainer>(x => _hangfireEventHandlerContainer);

            services.AddHangfireSubPub<TestEvent>()
                        .Subscribe<TestHandler>();

            var provider = services.BuildServiceProvider();
            var IHangfireEventHandlerContainer = provider.GetRequiredService<IHangfireEventHandlerContainer>();

            _serviceProvider.Setup(s => s.GetRequiredService(typeof(TestHandler)))
                .Returns(new TestHandler());

            IHangfireEventHandlerContainer.Publish(new TestEvent
            {
                Name = "Bob"
            });

            _backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => job.Method.Name == "RunAsync"), It.IsAny<EnqueuedState>()));
        }
    }
}