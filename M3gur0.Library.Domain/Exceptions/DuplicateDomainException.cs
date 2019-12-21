using System;

namespace M3gur0.Library.Domain.Exceptions
{
    public class DuplicateDomainException : DomainException
    {
        public DuplicateDomainException(string objectKey, Type objectType) : base($"An item [{objectType?.Name}] with the same key [{objectKey}] already exists.") { }
    }
}
