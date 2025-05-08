using Application.Common.Behavior;
using Application.Votes.factory;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(DependencyInjection).Assembly);

            services.AddFluentValidation(fv =>
                fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddScoped<VoteStrategyAbstractFactory, CommentVoteStrategyFactory>();
            services.AddScoped<VoteStrategyAbstractFactory, PostVoteStrategyFactory>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
