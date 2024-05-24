using Microsoft.AspNetCore.Mvc;
using Registration.Api.Events;
using Registration.Api.Models;
using SubPub.Hangfire;

namespace Registration.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IHangfireEventHandlerContainer _hangfireEventHandlerContainer;

        public RegistrationController(IHangfireEventHandlerContainer hangfireEventHandlerContainer)
        {
            _hangfireEventHandlerContainer = hangfireEventHandlerContainer;
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            var registerEvent = new RegisterEvent
            {
                Email = model.Email,
                Date = DateTimeOffset.Now,
            };

            _hangfireEventHandlerContainer.Publish(registerEvent);

            return Ok();
        }

        [HttpPost("royal")]
        public IActionResult RegisterRoyal(RegisterModel model)
        {
            var registerEvent = new RoyalRegisterEvent
            {
                Email = model.Email,
                Date = DateTimeOffset.Now,
            };

            _hangfireEventHandlerContainer.Publish(registerEvent);

            return Ok();
        }

        [HttpPost("base")]
        public IActionResult RegisterBase(RegisterModel model)
        {
            var registerEvent = new BaseRegisterEvent
            {
                PhysicalAddress = "Somewhere peaceful",
                PostalAddress = "12 peaceful street, Nice Place 4001",
            };

            _hangfireEventHandlerContainer.Publish(registerEvent);

            return Ok();
        }

        [HttpPost("schedule")]
        public IActionResult RegisterSchedule(RegisterModel model)
        {
            var registerEvent = new RegisterEvent
            {
                Email = model.Email,
                Date = DateTimeOffset.Now,
            };

            var options = new HangfireJobOptions
            {
                HangfireJobType = HangfireJobType.Schedule,
                TimeSpan = TimeSpan.FromSeconds(15)
            };

            _hangfireEventHandlerContainer.Publish(registerEvent, options);

            return Ok();
        }
    }
}