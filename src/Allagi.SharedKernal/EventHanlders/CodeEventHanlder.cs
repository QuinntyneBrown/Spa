using Allagi.SharedKernal.Services;
using MediatR;
using Spa.Core.Builders.Component.Table;
using Spa.Core.Events;
using Spa.Core.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Core.EventHanlders
{
    public class CodeEventHanlder : INotificationHandler<CodeRequest>
    {
        private readonly IOrchestrationHandler _orchestrationHandler;
        private readonly ITemplateProcessor _templateProcessor;
        private readonly ITemplateLocator _templateLocator;

        public CodeEventHanlder(IOrchestrationHandler orchestrationHandler, ITemplateLocator templateLocator, ITemplateProcessor templateProcessor)
        {
            _orchestrationHandler = orchestrationHandler;
            _templateLocator = templateLocator;
            _templateProcessor = templateProcessor;
        }
        public async Task Handle(CodeRequest notification, CancellationToken cancellationToken)
        {
            await _orchestrationHandler.Publish(notification.TemplateName switch {
                "table" => new CodeResponse(TableGenerationStrategy.Build(notification.Prefix, _templateLocator, _templateProcessor)),
                "dialog" => new CodeResponse(DialogGenerationStrategy.Build(notification.Prefix, _templateLocator, _templateProcessor))
            });
        }
    }
}
