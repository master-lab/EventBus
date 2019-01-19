using Masterlab.EventBusTests.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterlab.EventBusTests.Subscribers
{
  public class AllEventsSubsciber
  {
    public void OnEvent(TestEvent @event) { }
    public void OnEvent(ImplementedEvent @event) { }
  }
}
