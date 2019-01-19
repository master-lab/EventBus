using Masterlab.EventBus.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Masterlab.EventBus
{
  public class EventBus : IEventBus
  {
    private static EventBus _defaultInstance = null;
    private SubscriberManager _subscriberManager = new SubscriberManager();
    private EventManager _eventManager = new EventManager();
    private ILogger _logger = new Logger();

    public static IEventBus DefaultInstance()
    {
      if(_defaultInstance == null)
      {
        _defaultInstance = new EventBus();
      }
      return _defaultInstance;
    }

    public EventBus()
    {
      _eventManager.OnEventDue(@event => Post(@event));
    }

    /// <summary>
    /// <para>Set delegate for logging. No logging will occur if log action is not set.</para>
    /// SetLogAction(message => logger.Debug(message));
    /// </summary>
    /// <param name="log"></param>
    public IEventBus SetLogAction(Action<string> log)
    {
      _logger.LogAction(log);
      _eventManager.SetLogAction(log);
      return this;
    }

    /// <summary>
    /// Set a repository for delayed events if persisted events are needed (through app restarts and such).
    /// If no repository is set, an in memory repository will be used for delayed events.
    /// </summary>
    /// <param name="eventRepository"></param>
    /// <returns></returns>
    public IEventBus SetEventRepository(IEventRepository eventRepository)
    {
      _eventManager.EventRepository = eventRepository;
      return this;
    }
    
    public void Post(object @event, Nullable<DateTime> executeDateTime_UTC = null, string eventKey = null)
    {
      log(@event, executeDateTime_UTC);
      if (!executeDateTime_UTC.HasValue || executeDateTime_UTC.Value <= DateTime.UtcNow)
      {
        postEvent(@event);
      }
      else
      {
        _eventManager.AddEvent(@event, executeDateTime_UTC.Value, eventKey);
      }
    }

    private void log(object @event, Nullable<DateTime> executeDateTime_UTC)
    {
      try
      {
        StackTrace stack = new StackTrace();
        var callingMethod = stack.GetFrame(2).GetMethod().ReflectedType;
        var executeAt = executeDateTime_UTC.HasValue ? string.Format("for execution at {0}", executeDateTime_UTC.Value) : "";
        _logger.Log(string.Format("{0} posted by {1} {2}", @event.GetType().Name, callingMethod, executeAt));
      }
      catch (Exception) { }
    }

    public void Cancel(string eventKey)
    {
      _eventManager.RemoveEvent(eventKey);
    }

    public void Register(object subscriber)
    {
      _subscriberManager.AddSubscriber(subscriber);
    }

    public void Unregister(object subscriber)
    {
      _subscriberManager.RemoveSubscriber(subscriber);
    }

    private void postEvent(object @event)
    {
      var subscribers = _subscriberManager.GetSubscribersFor(@event);
      foreach (var sub in subscribers)
      {
        var subscriberMethod = sub.Value;
        var subscriberObj = sub.Key;
        try
        {
          Task.Run(() =>
          {
            _logger.Log(string.Format("{0} posted to {1}", @event.GetType().Name, subscriberObj.GetType().FullName));
            subscriberMethod.Invoke(subscriberObj, new object[] { @event });
          });
        }
        catch (Exception ex)
        {
          _logger.Log(string.Format("Error invoking method on {0} for event {1}", subscriberObj.GetType().FullName, @event.GetType().Name));
        }
      }
    }

  }
}
