namespace Registration.Api.Events
{
    public class RegisterEvent
    {
        public string Email { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}