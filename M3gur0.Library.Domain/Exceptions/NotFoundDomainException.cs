using System;

namespace M3gur0.Library.Domain.Exceptions
{
    public class NotFoundDomainException : Exception
    {
        public NotFoundDomainException(string objectKey, Type objectType) : base($"There is no item [{objectType?.Name}] found with the following key {objectKey}") { }
    }
}
