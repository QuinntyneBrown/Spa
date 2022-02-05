using Spa.Core.Models;
using Spa.Core.Services;

namespace Spa.Core.Builders.Component.Table
{
    public class TableBuilder
    {
        private static readonly string TEMPLATE_NAME = "Table";
        public static AngularCodeResult Build(string prefix, ITemplateLocator templateLocator, ITemplateProcessor templateProcessor)
        {
            var tokens = new TokensBuilder()
                .With(nameof(prefix),(Token)prefix)
                .Build();

            return new AngularCodeResult
            {
                Html = templateProcessor.Process(templateLocator.Get($"{TEMPLATE_NAME}Html"), tokens),
                TypeScript = templateProcessor.Process(templateLocator.Get($"{TEMPLATE_NAME}TypeScript"), tokens),
                Scss = templateProcessor.Process(templateLocator.Get($"{TEMPLATE_NAME}Scss"), tokens),
            };
        }
    }
}
