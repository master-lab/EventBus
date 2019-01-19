using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterlab.EventBusTests.Events
{
  public class TestEvent : IEvent
  {
    public string EventInstance { get; set; }
  }
}
