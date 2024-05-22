using Hangfire;
using Hangfire.Storage.SQLite;
using Events.Hangfire.SubPub;
using Registration.Api.Events;
using Registration.Api.Handlers;

namespace Registration.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHangfire(x => x.UseSQLiteStorage());
            builder.Services.AddHangfireServer();

            builder.Services.AddHangfireSubPub<RegisterEvent>()
                            .Subscribe<RegisterHandler>()
                            .Subscribe<Register2Handler>();

            builder.Services.AddHangfireSubPub<DuplicateRegisterEvent>()
                            .Subscribe<DuplicateRegisterHandler>();

            builder.Services.AddHangfireSubPub<BaseRegisterEvent>()
                            .Subscribe<BaseRegisterHandler>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            app.UseHangfireDashboard();

            app.Run();
        }
    }
}