using Microsoft.Extensions.DependencyInjection;
using ServiceTemplate.Application.Engines;
using ServiceTemplate.Application.Interfaces;
using ServiceTemplate.Application.Orchestrators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTemplate.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<IServiceOrchestrator, ServiceOrchestrator>();
            services.AddSingleton<IServiceEngine, ServiceEngine>();

            return services;
        }
    }
}
