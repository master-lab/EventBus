using Masterlab.EventBusTests.Events;

namespace Masterlab.EventBusTests.Subscribers
{
  public class TestEventSubscriber
  {
    public void OnEvent(TestEvent @event) { }
  }
}
