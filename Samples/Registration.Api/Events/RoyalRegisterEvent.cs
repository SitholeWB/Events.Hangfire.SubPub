namespace Registration.Api.Events
{
    public class RoyalRegisterEvent : BaseRegisterEvent
    {
        public string Email { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}