using M3gur0.Library.Domain.Exceptions;
using System.Net;

namespace M3gur0.Library.WebAPI.ProblemDetails
{
    public class InvalidRequestProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        public InvalidRequestProblemDetails(RequestValidationException exception)
        {
            Title = "Invalid request";
            Status = (int)HttpStatusCode.BadRequest;
            Detail = exception?.Message;
            Type = "https://library.m3gur0.com/validation-error";
        }
    }
}
