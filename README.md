# SubPub.Hangfire

```

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

    //builder.Services.AddHangfire(x => x.UseSQLiteStorage());
    //builder.Services.AddHangfireServer();

    builder.Services.AddHangfireSubPub<RegisterEvent>()
                    .Subscribe<RegisterHandler>()
                    .Subscribe<Register2Handler>();

    builder.Services.AddHangfireSubPub<DuplicateRegisterEvent>()
                    .Subscribe<DuplicateRegisterHandler>();

```