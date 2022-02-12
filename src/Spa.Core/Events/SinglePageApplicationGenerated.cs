using MediatR;
using Spa.Core.Models;

namespace Spa.Core.Events
{
    public class SinglePageApplicationGenerated: INotification
    {
        public SinglePageApplicationGenerated(Settings settings)
        {
            Settings = settings;
        }
        public Settings Settings { get; private set; }
    }
}
