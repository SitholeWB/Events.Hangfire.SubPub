namespace Registration.Api.Events
{
    public class BaseRegisterEvent
    {
        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }
    }
}