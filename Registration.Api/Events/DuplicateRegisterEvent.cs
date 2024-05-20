namespace Registration.Api.Events
{
    public class DuplicateRegisterEvent
    {
        public string Email { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}