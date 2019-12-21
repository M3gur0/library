using M3gur0.Library.Domain.Exceptions;
using System.Net;

namespace M3gur0.Library.WebAPI.ProblemDetails
{
    public class UnknownEntryProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        public UnknownEntryProblemDetails(NotFoundDomainException exception)
        {
            Title = "Item not found";
            Status = (int)HttpStatusCode.NotFound;
            Detail = exception?.Message;
            Type = "https://library.m3gur0.com/validation-error";
        }
    }
}
