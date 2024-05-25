# SubPub.Hangfire

Lets you create applications with event publishers and subscribers.
Publishers communicate LOCALLY with subscribers by broadcasting events, Hangfire will process these events in the background via implemented handlers

```nuget
Install-Package SubPub.Hangfire
```

```C#

    public class RegisterEvent
    {
        public string Email { get; set; }
        public DateTimeOffset Date { get; set; }
    }
    public class DuplicateRegisterEvent
    {
        public string Email { get; set; }
        public DateTimeOffset Date { get; set; }
    }

    public class RegisterHandler : IHangfireEventHandler<RegisterEvent>
    {
        public Task RunAsync(RegisterEvent obj)
        {
            Console.WriteLine("**************START 1*********");
            Console.WriteLine($"{obj.Email} from RegisterHandler");
            Console.WriteLine("**************END 1*********");

            return Task.CompletedTask;
        }
    }
    public class Register2Handler : IHangfireEventHandler<RegisterEvent>
    {
        public Task RunAsync(RegisterEvent obj)
        {
            Console.WriteLine("**************START 1*********");
            Console.WriteLine($"{obj.Email} from Register2Handler");
            Console.WriteLine("**************END 1*********");

            return Task.CompletedTask;
        }
    }

    public class DuplicateRegisterHandler : IHangfireEventHandler<DuplicateRegisterEvent>
    {
        public Task RunAsync(DuplicateRegisterEvent obj)
        {
            Console.WriteLine("**************START 1*********");
            Console.WriteLine($"{obj.Email} from DuplicateRegisterHandler");
            Console.WriteLine("**************END 1*********");

            return Task.CompletedTask;
        }
    }

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
    }


    //builder.Services.AddHangfire(x => x.UseSQLiteStorage());
    //builder.Services.AddHangfireServer();

    builder.Services.AddHangfireSubPub<RegisterEvent>()
                    .Subscribe<RegisterHandler>()
                    .Subscribe<Register2Handler>();

    builder.Services.AddHangfireSubPub<DuplicateRegisterEvent>()
                    .Subscribe<DuplicateRegisterHandler>();

```
## Credit
I got some of the other code ideas from this repository https://github.com/lamondlu/EventHandlerInSingleApplication
