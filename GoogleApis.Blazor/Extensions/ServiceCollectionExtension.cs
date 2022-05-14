using GoogleApis.Blazor.Auth;
using GoogleApis.Blazor.Calendar;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApis.Blazor.Extensions
{
    /// <summary>
    /// A static class contains codebeam google api services. The lifetime is scoped.
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds all codebeam google services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCodeBeamGoogleApiServices(this IServiceCollection services)
        {
            return services
                .AddCodeBeamGoogleAuthService()
                .AddCodeBeamGoogleCalendarService();
        }

        /// <summary>
        /// Adds codebeam google auth service as scoped.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCodeBeamGoogleAuthService(this IServiceCollection services)
        {
            services.TryAddScoped<AuthService>();
            return services;
        }

        /// <summary>
        /// Adds codebeam google calendar service as scoped.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCodeBeamGoogleCalendarService(this IServiceCollection services)
        {
            services.TryAddScoped<CalendarService>();
            return services;
        }

    }
}
