using Autofac;
using System.Collections.Generic;

namespace Playground.Core.MessageBus
{
    public class EventHandlerBuilder
    {
        private readonly ILifetimeScope _scope;

        public EventHandlerBuilder(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public void HandleEvents<T>(T message) where T : IEvent
        {
            var handlers = _scope.Resolve<IEnumerable<IEventHandler<T>>>();
            foreach(var handler in handlers)
            {
                handler.Handle(message);
            }
        }
    }
}
