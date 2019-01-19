using Masterlab.EventBusTests.Events;

namespace Masterlab.EventBusTests.Subscribers
{
  public class IEventSubscriber
  {
    public void OnEvent(IEvent @event) { }
  }
}
