using Masterlab.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterlab.EventBusTests.Repository
{
  public class MockEventRepository : IEventRepository
  {
    private class EventHolder
    {
      public EventHolder(DateTime dateTime, object @event)
      {
        executeDateTime = dateTime;
        eventObj = @event;
      }

      public DateTime executeDateTime { get; set; }
      public object eventObj { get; set; }
    }

    private IDictionary<string, EventHolder> _eventStore = new Dictionary<string, EventHolder>();

    public void AddEvent(object @event, DateTime executeDateTime_UTC, string eventKey)
    {
      _eventStore.Add(eventKey, new EventHolder(executeDateTime_UTC, @event));
    }

    public IEnumerable<object> GetEvents()
    {
      return _eventStore.Select(o => o.Value.eventObj).ToList();
    }

    public IDictionary<string, object> GetEventsDue(DateTime dateTime_UTC)
    {
      return _eventStore.Where(o => o.Value.executeDateTime <= DateTime.UtcNow).ToDictionary(o => o.Key, o => o.Value.eventObj);
    }

    public Nullable<DateTime> GetDateTimeOfNextEventDue()
    {
      Nullable<DateTime> retVal = null;
      if (_eventStore.Count > 0)
      {
        retVal = _eventStore.Select(o => o.Value.executeDateTime).OrderBy(d => d).FirstOrDefault();
      }
      return retVal;
    }

    public void RemoveEvent(string eventKey)
    {
      if (_eventStore.ContainsKey(eventKey))
      {
        _eventStore.Remove(eventKey);
      }
    }
  }
}
