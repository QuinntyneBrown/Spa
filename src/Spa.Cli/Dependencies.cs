using Allagi.SharedKernal.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Spa.Core;
using Spa.Core.Services;

namespace Spa.Cli
{
    public static class Dependencies
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddMediatR(typeof(Allagi.SharedKernal.Constants),typeof(Marker));
            services.AddSingleton<ICommandService, CommandService>();
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<ITemplateLocator, TemplateLocator>();
            services.AddSingleton<ITemplateProcessor, LiquidTemplateProcessor>();
            services.AddSingleton<INamingConventionConverter, NamingConventionConverter>();
            services.AddSingleton<ISettingsProvider, SettingsProvider>();
            services.AddSingleton<ITenseConverter, TenseConverter>();
            services.AddSingleton<IContext, Context>();
            services.AddSingleton<IAngularJsonProvider, AngularJsonProvider>();
            services.AddSingleton<INearestModuleNameProvider, NearestModuleNameProvider>();
            services.AddSingleton<IPackageJsonService, PackageJsonService>();
            services.AddSingleton<IOrchestrationHandler, OrchestrationHandler>();
        }
    }
}
