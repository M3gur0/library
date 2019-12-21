using M3gur0.Library.Domain.Exceptions;
using System.Net;

namespace M3gur0.Library.WebAPI.ProblemDetails
{
    public class DuplicateEntryProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        public DuplicateEntryProblemDetails(DuplicateDomainException exception)
        {
            Title = "Item already exists";
            Status = (int)HttpStatusCode.Conflict;
            Detail = exception?.Message;
            Type = "https://library.m3gur0.com/validation-error";
        }
    }
}
