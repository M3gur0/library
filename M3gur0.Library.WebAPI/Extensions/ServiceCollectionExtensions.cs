using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Hellang.Middleware.ProblemDetails;
using M3gur0.Library.Domain;
using M3gur0.Library.Domain.Exceptions;
using M3gur0.Library.Infrastructure.Data.EF;
using M3gur0.Library.WebAPI.Modules;
using M3gur0.Library.WebAPI.ProblemDetails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Text.Json.Serialization;

namespace M3gur0.Library.WebAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomDbContext<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsBuilder)
            where TContext : DbContext
        {
            services
                .AddTransient(typeof(DbContext), typeof(TContext))
                .AddTransient(typeof(IReadWriteRepository<>), typeof(ReadWriteRepository<>))
                .AddTransient(typeof(IReadOnlyRepository<>), typeof(ReadOnlyRepository<>))
                .AddDbContext<TContext>(optionsBuilder, ServiceLifetime.Scoped);

            return services;
        }

        public static IServiceCollection AddMediator<TMediatorAssembly>(this IServiceCollection services) => services.AddMediatR(typeof(TMediatorAssembly));

        public static IServiceCollection AddCustomMappers(this IServiceCollection services, params Type[] assemblyTypes) => services.AddAutoMapper(assemblyTypes);

        public static IServiceCollection AddSwagger(this IServiceCollection services, OpenApiInfo openApiInfo) => services.AddSwaggerGen(options => options.SwaggerDoc(openApiInfo.Version, openApiInfo));

        public static IServiceCollection AddCustomAPIBehavior(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddProblemDetails(options =>
            {
                options.Map<RequestValidationException>(ex => new InvalidRequestProblemDetails(ex));
                options.Map<DuplicateDomainException>(ex => new DuplicateEntryProblemDetails(ex));
                options.Map<NotFoundDomainException>(ex => new UnknownEntryProblemDetails(ex));
                options.Map<Exception>(ex => new ServerErrorProblemDetails(ex));
            });

            return services;
        }

        public static IServiceProvider Build(this IServiceCollection services, params Module[] modules)
        {
            var container = new ContainerBuilder();
            container.Populate(services);
            container.RegisterModule<MediatRModule>();

            if (modules != null)
            {
                foreach (var module in modules)
                {
                    container.RegisterModule(module);
                }
            }

            return new AutofacServiceProvider(container.Build());
        }
    }
}
