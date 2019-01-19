using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Masterlab.EventBusTests")]
namespace Masterlab.EventBus
{
  internal class SubscriberManager
  {
    private static readonly string ON_EVENT = "OnEvent";
    private ICollection<WeakReference<object>> _subscribers = new HashSet<WeakReference<object>>();

    public IDictionary<object, MethodInfo> GetSubscribersFor(object @event)
    {
      IDictionary<object, MethodInfo> retVal = new Dictionary<object, MethodInfo>();
      lock (_subscribers)
      {
        ICollection<object> subscribers = GetSubscribers();
        foreach (var subscriber in subscribers)
        {
          MethodInfo methodInfo = GetMethodInfo(@event, subscriber);
          if (methodInfo != null)
          {
            retVal.Add(subscriber, methodInfo);
          }
        }
      }
      return retVal;
    }

    public void AddSubscriber(object subscriber)
    {
      lock (_subscribers)
      {
        if (!_subscribers.Contains(subscriber))
        {
          _subscribers.Add(new WeakReference<object>(subscriber));
        }
      }
    }

    public void RemoveSubscriber(object subscriber)
    {
      lock (_subscribers)
      {
        var weakSubscriber = GetSubscriber(subscriber);
        if (weakSubscriber != null)
        {
          _subscribers.Remove(weakSubscriber);
        }
      }
    }

    private MethodInfo GetMethodInfo(object @event, object subscriber)
    {
      Type type = subscriber.GetType();
      var methods = type.GetRuntimeMethods();
      MethodInfo methodInfo = type.GetRuntimeMethod(ON_EVENT, new Type[] { @event.GetType() });
      return methodInfo;
    }
    
    private ICollection<object> GetSubscribers()
    {
      ICollection<object> retVal = new HashSet<object>();
      ICollection<WeakReference<object>> nullSubscribers = new HashSet<WeakReference<object>>();
      foreach (var weakSubscriber in _subscribers)
      {
        var subscriber = GetSubscriber(weakSubscriber);
        if (subscriber == null)
        {
          nullSubscribers.Add(weakSubscriber);
        }
        else
        {
          retVal.Add(subscriber);
        }
      }

      // remove GC collected references
      foreach (var weakSubscriber in nullSubscribers)
      {
        _subscribers.Remove(weakSubscriber);
      }

      return retVal;
    }

    private WeakReference<object> GetSubscriber(object subscriber)
    {
      WeakReference<object> retVal = null;
      foreach (var weakSubscriber in _subscribers)
      {
        var concreteSubscriber = GetSubscriber(weakSubscriber);
        if (concreteSubscriber != null && concreteSubscriber == subscriber)
        {
          retVal = weakSubscriber;
          break;
        }
      }
      return retVal;
    }

    private object GetSubscriber(WeakReference<object> weakSubscriber)
    {
      object subscriber = null;
      weakSubscriber.TryGetTarget(out subscriber);
      return subscriber;
    }

  }
}
