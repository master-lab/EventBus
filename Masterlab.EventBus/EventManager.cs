using Masterlab.EventBus.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Masterlab.EventBus
{
  public class EventManager
  {
    private IEventRepository _eventRepository;
    private static object REPO_LOCK = new object();
    private Action<object> _onEventDueAction;
    private CancellationTokenSource _dueEventCheckToken;
    protected Nullable<DateTime> _nextEventDue = null;
    private ILogger _logger = new Logger();

    public EventManager()
    {
      FetchAndHandleDueEvents();
    }

    public IEnumerable<object> GetEventsDue()
    {
      lock (REPO_LOCK)
      {
        _logger.Log("EventManager getting due events");
        var dueEvents = EventRepository.GetEventsDue(DateTime.UtcNow);
        foreach(string key in dueEvents.Keys)
        {
          EventRepository.RemoveEvent(key);
        }
        return dueEvents.Values;
      }
    }

    public Nullable<DateTime> GetDateTimeOfNextEventDue()
    {
      lock (REPO_LOCK)
      {
        return EventRepository.GetDateTimeOfNextEventDue();
      }
    }

    public IEnumerable<object> GetEvents()
    {
      lock (REPO_LOCK)
      {
        return EventRepository.GetEvents();
      }
    }

    public void AddEvent(object @event, DateTime executeDateTime_UTC, string eventKey = null)
    {
      lock (REPO_LOCK)
      {
        if (string.IsNullOrEmpty(eventKey))
        {
          eventKey = generateEventKey();
        } 
        else
        {
          // remove any event that may already exist with same event key
          EventRepository.RemoveEvent(eventKey);
        }

        EventRepository.AddEvent(@event, executeDateTime_UTC, eventKey);
        ScheduleNextDelayedEvent(executeDateTime_UTC);
      }
    }

    public void RemoveEvent(string eventKey)
    {
      lock (REPO_LOCK)
      {
        EventRepository.RemoveEvent(eventKey);
      }
    }

    public void OnEventDue(Action<object> onEventDue)
    {
      _onEventDueAction = onEventDue;
    }

    public void SetLogAction(Action<string> log)
    {
      _logger.LogAction(log);
    }

    public IEventRepository EventRepository
    {
      private get
      {
        if (_eventRepository == null) { _eventRepository = new EventRepository(); }
        return _eventRepository;
      }
      set
      {
        _eventRepository = value;
        FetchAndHandleDueEvents();
      }
    }

    private string generateEventKey()
    {
      return string.Format("_{0}_", Guid.NewGuid().ToString().Substring(0, 6));
    }
    
    private void FetchAndHandleDueEvents()
    {
      var dueEvents = GetEventsDue();
      if (_onEventDueAction != null)
      {
        foreach (var e in dueEvents)
        {
          _onEventDueAction(e);
        }
      }

      ScheduleNextDelayedEvent();
    }

    private async Task ScheduleNextDelayedEvent(Nullable<DateTime> executeDateTime_UTC = null)
    {
      try
      {
        if (!_nextEventDue.HasValue || (executeDateTime_UTC.HasValue && _nextEventDue.Value > executeDateTime_UTC.Value))
        {
          CancelEventSchedule(); 
          Nullable<DateTime> next = GetDateTimeOfNextEventDue();
          if (next.HasValue)
          {
            int seconds = 1;
            if (next >= DateTime.UtcNow) { seconds = Convert.ToInt32((next.Value - DateTime.UtcNow).TotalSeconds); }
            _nextEventDue = next.Value;
            _logger.Log(string.Format("EventManager scheduled next event in {0} seconds.", seconds));
            _dueEventCheckToken = new CancellationTokenSource();
            await Task.Delay(seconds * 1000, _dueEventCheckToken.Token);
            _nextEventDue = null;
            FetchAndHandleDueEvents();
          }
        }
      }
      catch (TaskCanceledException ex)
      {
        // cancelled task
      }
    }

    private void CancelEventSchedule()
    {
      if (_nextEventDue.HasValue)
      {
        try
        {
          _nextEventDue = null;
          _dueEventCheckToken.Cancel();
          _logger.Log("EventManager cancelled scheduled event check");
        }
        catch (TaskCanceledException e)
        {
          _logger.Log("EventManager TaskCanceledException. " + e.Message);
        }
      }
    }

  }
}
