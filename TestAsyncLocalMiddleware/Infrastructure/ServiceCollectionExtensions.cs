using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.NLog;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TestAsyncLocalMiddleware.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContextFromRequest(this IServiceCollection services)
        {
            services.AddSingleton<ISecurityTokenAccessor, SecurityTokenAccessor>();

            return services.AddScoped(sp =>
            {
                var securityTokenAccessor = sp.GetRequiredService<ISecurityTokenAccessor>();
                var headers = sp.GetService<IHttpContextAccessor>()?.HttpContext?.Request.Headers
                    .ToDictionary(h => h.Key, h => h.Value.AsEnumerable());


                return ContextBuilder.BuildContext(securityTokenAccessor, headers);
            });
        }

        public static IServiceCollection AddCommonLogging(this IServiceCollection services)
        {
            var properties = new NameValueCollection();
            var nlogAdapter = new NLogLoggerFactoryAdapter(properties);
            LogManager.Adapter = nlogAdapter;

            var config = new NLog.Config.LoggingConfiguration();
            config.AddTarget("console", new ConsoleTarget
            {
                Layout = "${my-layout}"
            });
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, "console");
            NLog.Config.ConfigurationItemFactory.Default.RegisterItemsFromAssembly(Assembly.GetExecutingAssembly());

            NLog.LogManager.Configuration = config;

            return services.AddScoped<ILog>(sp =>
            {
                var context = sp.GetRequiredService<DefaultServiceContext>();

                // inject context info to Logger
                NLog.MappedDiagnosticsLogicalContext.Set("corp-id", context.CorpId);
                NLog.MappedDiagnosticsLogicalContext.Set("user-id", context.UserId);

                return LogManager.GetLogger("app");

            });
        }
    }
}
