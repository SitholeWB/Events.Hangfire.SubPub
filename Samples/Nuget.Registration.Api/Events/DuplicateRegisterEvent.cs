namespace Nuget.Registration.Api.Events
{
    public class DuplicateRegisterEvent : BaseRegisterEvent
    {
        public string Email { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}