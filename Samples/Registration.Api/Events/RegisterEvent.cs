namespace Registration.Api.Events
{
    public class RegisterEvent : BaseRegisterEvent
    {
        public string Email { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}