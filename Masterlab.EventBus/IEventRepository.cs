using System;
using System.Collections.Generic;

namespace Masterlab.EventBus
{
  public interface IEventRepository
  {
    /// <summary>
    /// Add delayed event to event store.  Any event that exists with the
    /// same eventKey will be removed before adding the new event.
    /// </summary>
    /// <param name="@event">event object to add</param>
    /// <param name="executeDateTime_UTC">datetime to execute event</param>
    /// <param name="eventKey">unique identifier for event</param>
    void AddEvent(object @event, DateTime executeDateTime_UTC, string eventKey);
    
    /// <summary>
    /// Get all events
    /// </summary>
    /// <returns></returns>
    IEnumerable<object> GetEvents();

    /// <summary>
    /// Get Dictionary of event keys and event objects that have execution DateTimes less than or equal to now (UTC)
    /// </summary>
    /// <param name="dateTime_UTC"></param>
    /// <returns></returns>
    IDictionary<string, object> GetEventsDue(DateTime dateTime_UTC);

    /// <summary>
    /// Get DateTime (UTC) of next event due so it can be scheduled
    /// </summary>
    /// <returns></returns>
    Nullable<DateTime> GetDateTimeOfNextEventDue();

    /// <summary>
    /// Remove event by event key
    /// </summary>
    /// <param name="eventKey"></param>
    void RemoveEvent(string eventKey);
  }
}