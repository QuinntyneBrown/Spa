using Allagi.SharedKernal.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spa.Core;
using Spa.Core.Services;
using Spa.Core.Strategies;
using System.Reflection;

namespace Spa.Cli
{
    public static class Dependencies
    {
        public static void Configure(Assembly[] pluginAssemblies, IServiceCollection services)
        {
            services.AddMediatR(typeof(Allagi.SharedKernal.CoreConstants),typeof(Marker));
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
            services.AddSingleton<ISinglePageApplicationGenerationStrategyFactory, SinglePageApplicationGenerationStrategyFactory>();
            services.AddSingleton<ISettingsGenerationStrategyFactory, SettingsGenerationStrategyFactory>();
            services.AddSingleton<ITranslationAuditService, TranslationAuditService>();
            services.AddSingleton<ITranslationService, TranslationService>();

            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            services.AddSingleton<IConfiguration>(_ => configuration);
        }
    }
}
