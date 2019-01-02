using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Playground.Core.MessageBus
{
    public static class MessageBusExtension
    {
        public static IServiceCollection AddMessageBus(this IServiceCollection services, Action<BusConfig> busConfigAction)
        {
            var busConfig = new BusConfig();
            busConfigAction.Invoke(busConfig);

            var bus = new MessageBus(busConfig, services);
            services.AddSingleton<IMessageBus>(bus);

            return services;
        }

        public static IApplicationBuilder UseMessageBus(this IApplicationBuilder app)
        {
            var bus = app.ApplicationServices.GetRequiredService<IMessageBus>();
            bus.Connect();

            return app;
        }
    }
}
