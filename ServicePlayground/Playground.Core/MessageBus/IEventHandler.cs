namespace Playground.Core.MessageBus
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        void Handle(TEvent);
    }
}
