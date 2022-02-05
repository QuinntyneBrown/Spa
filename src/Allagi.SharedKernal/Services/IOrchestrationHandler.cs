using MediatR;
using System;
using System.Threading.Tasks;

namespace Allagi.SharedKernal.Services
{
    public interface IOrchestrationHandler
    {
        public Task Publish(IEvent message);
        public Task<T> Handle<T>(IEvent startWith, Func<TaskCompletionSource<T>, Action<INotification>> onNextFactory);
    }
}
