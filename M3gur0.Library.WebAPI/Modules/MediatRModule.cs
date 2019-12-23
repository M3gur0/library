using Autofac;
using FluentValidation;
using M3gur0.Library.Domain.Exceptions;
using M3gur0.Library.Infrastructure.Mediator.Behavior;
using MediatR;
using MediatR.Pipeline;
using System.Reflection;

namespace M3gur0.Library.WebAPI.Modules
{
    public class MediatRModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            var mediatorOpenTypes = new[]
            {
                typeof(IRequestHandler<>),
                typeof(INotificationHandler<>),
                typeof(IValidator<>)
            };

            foreach (var mediatorOpenType in mediatorOpenTypes)
            {
                builder
                    .RegisterAssemblyTypes(typeof(DuplicateDomainException).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(mediatorOpenType)
                    .AsImplementedInterfaces();
            }

            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.RegisterGeneric(typeof(RequestValidationBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}
