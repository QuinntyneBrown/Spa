using MediatR;
using Spa.Core.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spa.Application.Plugin.Scss.Handlers
{
    internal class ScssPluglinHandler : INotificationHandler<SinglePageApplicationGenerated>
    {
        public Task Handle(SinglePageApplicationGenerated notification, CancellationToken cancellationToken)
        {
            Console.Write("Handled");

            return Task.CompletedTask;
        }
    }
}
