using System;

namespace M3gur0.Library.Domain.Exceptions
{
    public class RequestValidationException : Exception
    {
        public RequestValidationException(string message) : base(message) { }
    }
}
