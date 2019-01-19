using Microsoft.VisualStudio.TestTools.UnitTesting;
using Masterlab.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masterlab.EventBusTests.Subscribers;
using Masterlab.EventBusTests.Events;
using System.Threading;

namespace Masterlab.EventBusTests
{
  [TestClass()]
  public class SubscriberManagerTests
  {
    [TestMethod()]
    public void AddSubscriberTest()
    {
      var manager = new SubscriberManager();

      var subscriber = new TestEventSubscriber();
      manager.AddSubscriber(subscriber);

      var subscribers = manager.GetSubscribersFor(new TestEvent());
      Assert.IsTrue(subscribers.Count == 1);

      var subscriber2 = new BaseEventSubscriber();
      manager.AddSubscriber(subscriber2);

      subscribers = manager.GetSubscribersFor(new ImplementedEvent(1));
      Assert.IsTrue(subscribers.Count == 1);
    }

    [TestMethod()]
    public void RemoveSubscriberTest()
    {
      var manager = new SubscriberManager();

      var subscriber = new TestEventSubscriber();
      var subscriber2 = new AllEventsSubsciber();
      manager.AddSubscriber(subscriber);
      manager.AddSubscriber(subscriber2);

      manager.RemoveSubscriber(subscriber);
      var subscribers = manager.GetSubscribersFor(new TestEvent());
      Assert.IsTrue(subscribers.Count == 1);
      
      manager.RemoveSubscriber(subscriber2);
      subscribers = manager.GetSubscribersFor(new TestEvent());
      Assert.IsTrue(subscribers.Count == 0);
      
    }

    [TestMethod()]
    public void GetSubscribersForTest()
    {
      var manager = new SubscriberManager();

      var testEventSubscriber = new TestEventSubscriber();
      var invalidSubscriber = new InvalidSubscriber();
      var baseEventSubscriber = new BaseEventSubscriber();
      var allEventsSubscriber = new AllEventsSubsciber();
      var ieventSubscriber = new IEventSubscriber();

      manager.AddSubscriber(testEventSubscriber);
      manager.AddSubscriber(allEventsSubscriber);
      var subscribers = manager.GetSubscribersFor(new TestEvent());
      Assert.IsTrue(subscribers.Count == 2);
      Assert.ReferenceEquals(subscribers.Keys.First(), testEventSubscriber);

      //test invalid subscribers
      manager.AddSubscriber(invalidSubscriber);
      subscribers = manager.GetSubscribersFor(new TestEvent());
      Assert.IsTrue(subscribers.Count == 2);

      //test base class subscriptions
      manager.AddSubscriber(baseEventSubscriber);
      subscribers = manager.GetSubscribersFor(new ImplementedEvent(1));
      Assert.IsTrue(subscribers.Count == 2);
      
      //test interface subscriptions
      manager.AddSubscriber(ieventSubscriber);
      subscribers = manager.GetSubscribersFor(new TestEvent());
      Assert.IsTrue(subscribers.Count == 3);
    }
  }
}