using Microsoft.VisualStudio.TestTools.UnitTesting;
using Masterlab.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Masterlab.EventBusTests.Events;

namespace Masterlab.EventBusTests
{
  [TestClass()]
  public class EventRepositoryTests
  {

    [TestMethod()]
    public void GetEventsDueTest()
    {
      var repo = new EventRepository();
      repo.AddEvent(new TestEvent(), DateTime.Now, "1");
      repo.AddEvent(new TestEvent(), DateTime.UtcNow.AddMinutes(15), "2");
      Assert.IsTrue(repo.GetEventsDue(DateTime.UtcNow).Count() == 1, "Expected 1 due event");
    }

    [TestMethod()]
    public void GetDateTimeOfNextEventDueTest()
    {
      var repo = new EventRepository();
      Assert.IsFalse(repo.GetDateTimeOfNextEventDue().HasValue);

      var next = DateTime.UtcNow.AddMinutes(15);
      repo.AddEvent(new TestEvent(), DateTime.UtcNow.AddMinutes(20), "1");
      repo.AddEvent(new TestEvent(), next, "2");
      repo.AddEvent(new TestEvent(), DateTime.UtcNow.AddMinutes(22), "3");
      Assert.IsTrue(repo.GetDateTimeOfNextEventDue().Value.Equals(next));
    }

    [TestMethod()]
    public void GetEventsTest()
    {
      var repo = new EventRepository();
      repo.AddEvent(new TestEvent(), DateTime.Now, "1");
      repo.AddEvent(new TestEvent(), DateTime.UtcNow.AddMinutes(15), "2");
      Assert.IsTrue(repo.GetEvents().Count() == 2, "Expected 2 events");
    }

    [TestMethod()]
    public void AddEventTest()
    {
      var repo = new EventRepository();
      Assert.IsTrue(repo.GetEvents().Count() == 0);

      repo.AddEvent(new TestEvent(), DateTime.Now, "1");
      Assert.IsTrue(repo.GetEvents().Count() == 1, "Failed to add event");

      var @event = new TestEvent();
      repo.AddEvent(@event, DateTime.UtcNow.AddMinutes(15), "e1");
      repo.AddEvent(@event, DateTime.UtcNow.AddMinutes(26), "e2");
      Assert.IsTrue(repo.GetEvents().Count() == 3, "Failed to add new event of same object instance with different eventKey");
    }

  }
}