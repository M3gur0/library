using System;
using System.Net;

namespace M3gur0.Library.WebAPI.ProblemDetails
{
    public class ServerErrorProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        public ServerErrorProblemDetails(Exception exception)
        {
            Title = exception?.Message;
            Status = (int)HttpStatusCode.InternalServerError;
            Type = "https://library.m3gur0.com/internal-error";
        }
    }
}
