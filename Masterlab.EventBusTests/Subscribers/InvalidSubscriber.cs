using Masterlab.EventBusTests.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterlab.EventBusTests.Subscribers
{
  public class InvalidSubscriber
  {
    public void OnEvent(TestEvent @event, string anotherParam) { }
    private void OnEvent(ImplementedEvent @event) { }
    public void OnEventNameChange(TestEventTwo @event) { }
  }
}
