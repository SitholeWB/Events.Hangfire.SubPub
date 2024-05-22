# Hangfire.SubPub

```

    public class RegisterEvent : BaseRegisterEvent
    {
        public string Email { get; set; }
        public DateTimeOffset Date { get; set; }
    }

    public class RegisterHandler : IHangfireEventHandler<RegisterEvent>
    {
        public Task RunAsync(RegisterEvent obj)
        {
            Console.WriteLine("**************START 1*********");
            Console.WriteLine($"{obj.Email} has registered");
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

    builder.Services.AddHangfireSubPub<BaseRegisterEvent>()
                            .Subscribe<BaseRegisterHandler>();
```