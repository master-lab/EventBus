using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Masterlab.EventBusTests")]
namespace Masterlab.EventBus
{
  internal class EventRepository : IEventRepository
  {
    // in memory storage for delayed events
    private IDictionary<string, EventHolder> _eventStore = new Dictionary<string, EventHolder>();

    public IDictionary<string, object> GetEventsDue(DateTime dateTime_UTC)
    {
      return _eventStore.Where(o => o.Value.executeDateTime_UTC <= dateTime_UTC).ToDictionary(o => o.Key, o => o.Value.eventObj);
    }

    public Nullable<DateTime> GetDateTimeOfNextEventDue()
    {
      Nullable<DateTime> retVal = null;
      if(_eventStore.Count > 0)
      {
        retVal = _eventStore.Select(o => o.Value.executeDateTime_UTC).OrderBy(d => d).FirstOrDefault();
      }
      return retVal;
    }

    public IEnumerable<object> GetEvents()
    {
      return _eventStore.Select(o => o.Value.eventObj);
    }

    public void AddEvent(object @event, DateTime executeDateTime_UTC, string eventKey)
    {
      _eventStore.Add(eventKey, new EventHolder(executeDateTime_UTC, @event));
    }

    public void RemoveEvent(string eventKey)
    {
      if (!string.IsNullOrEmpty(eventKey) && _eventStore.ContainsKey(eventKey))
      {
        _eventStore.Remove(eventKey);
      }
    }

    private class EventHolder
    {
      public EventHolder(DateTime dateTime_UTC, object @event)
      {
        this.executeDateTime_UTC = dateTime_UTC;
        eventObj = @event;
      }
      public DateTime executeDateTime_UTC { get; set; }
      public object eventObj { get; set; }
    }

  }
}
