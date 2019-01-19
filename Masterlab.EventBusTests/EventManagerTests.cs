using Microsoft.VisualStudio.TestTools.UnitTesting;
using Masterlab.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Masterlab.EventBusTests.Events;
using Masterlab.EventBusTests.Repository;

namespace Masterlab.EventBusTests
{
  [TestClass()]
  public class EventManagerTests
  {
    
    [TestMethod()]
    public void GetEventsDueTest()
    {
      var manager = new EventManager();
      IEventRepository repo = new MockEventRepository();
      manager.EventRepository = repo;

      manager.AddEvent(new TestEvent(), DateTime.Now);
      manager.AddEvent(new TestEvent(), DateTime.UtcNow.AddMinutes(15));
      Assert.IsTrue(repo.GetEventsDue(DateTime.UtcNow).Count() == 1, "Expected repo to have 1 due event");
      Assert.IsTrue(manager.GetEventsDue().Count() == 1, "Expected EventManager to have 1 due event");
      Assert.IsTrue(manager.GetEvents().Count() == 1, "Expected 1 event to be removed");
    }

    [TestMethod()]
    public void GetEventsTest()
    {
      var manager = new EventManager();
      IEventRepository repo = new MockEventRepository();
      manager.EventRepository = repo;

      manager.AddEvent(new TestEvent(), DateTime.Now);
      manager.AddEvent(new TestEvent(), DateTime.UtcNow.AddMinutes(15));
      Assert.IsTrue(repo.GetEvents().Count() == 2, "Expected 2 events");
    }

    [TestMethod()]
    public void AddEventTest()
    {
      var manager = new EventManager();
      IEventRepository repo = new MockEventRepository();
      manager.EventRepository = repo;

      manager.AddEvent(new object(), DateTime.Now);
      Assert.IsTrue(repo.GetEvents().Count() == 1, "Failed to add event");

      var @event = new TestEvent();
      manager.AddEvent(@event, DateTime.UtcNow.AddMinutes(15), "e1");
      manager.AddEvent(@event, DateTime.UtcNow.AddMinutes(25), "e1"); // this will replace the event just added
      Assert.IsTrue(repo.GetEvents().Count() == 2, "Failed to add event with same eventKey");
      manager.AddEvent(@event, DateTime.UtcNow.AddMinutes(26));
      Assert.IsTrue(repo.GetEvents().Count() == 3, "Failed to add new event of same object");
      
    }
  }
}