using FluentValidation;
using M3gur0.Library.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3gur0.Library.Infrastructure.Mediator.Behavior
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> requestValidators)
        {
            validators = requestValidators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var failures = validators
                .Select(v => v.Validate(request))
                .SelectMany(res => res.Errors)
                .Where(err => err != null)
                .ToList();

            if (failures.Any())
            {
                var errorBuilder = new StringBuilder();
                foreach (var failure in failures) errorBuilder.AppendLine(failure.ErrorMessage);
                throw new RequestValidationException(errorBuilder.ToString());
            }

            if (next == null) throw new ArgumentNullException(nameof(next));

            return await next().ConfigureAwait(false);
        }
    }
}
