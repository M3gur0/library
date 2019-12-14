namespace M3gur0.Library.Domain.Exceptions
{
    public class DuplicateDomainException : DomainException
    {
        public string Details { get; }

        public DuplicateDomainException(string message, string details) : base(message)
        {
            Details = details;
        }
    }
}
