using Masterlab.EventBus.Logging;
using System;

namespace Masterlab.EventBus
{
  public interface IEventBus
  {
    /// <summary>
    /// <para>Post an event to subscribers</para>
    /// <para>The (optional) eventKey must be unique if provided and is used for delayed events. </para>
    /// Delayed Events posted with an 
    /// eventKey will replace any existing Event with the same eventKey. This is convenient when
    /// wanting to further delay an already delayed event.
    /// </summary>
    /// <param name="@event">Event object to post</param>
    /// <param name="postDateTimeUTC">DateTime to deliver if delayed event</param>
    /// <param name="eventKey">For delayed events, this is the unique key used for removing the event</param>
    void Post(object @event, DateTime? postDateTimeUTC = default(DateTime?), string eventKey = null);
    /// <summary>
    /// Cancel a delayed event using it's eventKey
    /// </summary>
    /// <param name="eventKey"></param>
    void Cancel(string eventKey);
    void Register(object subscriber);
    void Unregister(object subscriber);
    /// <summary>
    /// <para>Set delegate for logging. No logging will occur if LogAction is not set.</para>
    /// LogAction(message => logger.Debug(message));
    /// </summary>
    /// <param name="log"></param>
    IEventBus SetLogAction(Action<string> logAction);
    IEventBus SetEventRepository(IEventRepository eventRepository);
  }
}