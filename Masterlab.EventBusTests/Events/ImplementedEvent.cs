using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterlab.EventBusTests.Events
{
  public class ImplementedEvent : BaseEvent
  {
    public ImplementedEvent(int id)
    {
      base.EventId = id;
    }
  }
}
