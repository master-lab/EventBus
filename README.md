EventBus
---------

EventBus is a C# class library providing Thread safe publish/subscribe functionality to C# projects. Simplify communication 
between Web components in a decoupled manner.  Subscribers just need to register with an IEventBus and 
provide OnEvent methods to handle events (any class, interface, or base class). 
```C#
// get instance to use/inject in subscriber
IEventBus eventBus = EventBus.DefaultInstance();

// register the subscriber
eventBus.Register(this);

// create subscriber methods
public void OnEvent(CustomerEvent @event);
public void OnEvent(IOrderEvent @event);

// post events from other classes/components
eventBus.Post(new CustomerEvent());
```

Customize Behavior
-----------------------

#### Logging

```C#
// provide logging action
eventBus.LogAction(msg => Console.WriteLine(msg));

// use your existing logger
eventBus.LogAction(msg => logger.Debug(msg));
```

#### Event Repository

Delayed events are stored in memory by default.  If delayed events need to survive app restarts and such, a custom repository may be provided.

Simply provide a class that implements Masterlab.EventBus.IEventRepository

```C#
  public class CustomEventRepository : IEventRepository
  {
    public void AddEvent(object @event, DateTime executeDateTime_UTC, string eventKey)
    {
      // Add event to database
    }

    public DateTime? GetDateTimeOfNextEventDue()
    {
      // Fetch DateTime of next event to fire
    }

    public IEnumerable<object> GetEvents()
    {
      // Get all events
    }

    public IDictionary<string, object> GetEventsDue(DateTime dateTime_UTC)
    {
      // Get dictionary of event keys and event objects
    }

    public void RemoveEvent(string eventKey)
    {
      // Remove event from database
    }
  }
```

Then register your event repository with the EventBus

```C#
eventBus.SetEventRepository(customEventRepo);
```

Usage
-----


Get or create EventBus instance

```C#
  // get default instance
  IEventBus eventBus = EventBus.DefaultInstance();

  // or create an instance for specific use
  IEventBus orderEventBus = new EventBus();
  
  // optionally customize EventBus when creating instance
  IEventBus eventBus = new EventBus()
    .SetLogAction(msg => Console.WriteLine(msg))
    .SetEventRepository(customEventRepo);
```

Create Events.
Events can be any object and can optionally implement an interface or extend a base class.

```C#
  public class OrderCreatedEvent : IOrderEvent
  {
    public OrderCreatedEvent(Guid orderNumber)
    {
      OrderNumber = orderNumber;
    }

    public Guid OrderNumber { get; set; }
  }
  
  public class OrderShippedEvent : IOrderEvent
  {
    public OrderShippedEvent(Guid orderNumber)
    {
      OrderNumber = orderNumber;
    }

    public Guid OrderNumber { get; set; }
  }
  
  public class CustomerCreatedEvent : CustomerEvent
  {
    public CustomerCreatedEvent(int customerId)
    {
      CustomerId = customerId;
      base.CreatedDateTime = DateTime.Now;
    }

    public int CustomerId { get; set; }
  }
```

Create subscribers/handlers

```C#
  public class OrderHandler
  {
    public OrderHandler(IEventBus eventBus)
    {
      eventBus.Register(this);
    }

    public void OnEvent(OrderCreatedEvent @event)
    {
      Console.WriteLine("OrderHandler => OrderCreatedEvent {0}", @event.OrderNumber);
    }

    public void OnEvent(OrderShippedEvent @event)
    {
      Console.WriteLine("OrderHandler => OrderShippedEvent {0}", @event.OrderNumber);
    }
    
  }
```

Post events from anywhere

```C#
  public class OrderManager
  {
    private IEventBus _eventBus;

    public OrderManager(IEventBus eventBus)
    {
      _eventBus = eventBus;
    }

    public void ProcessOrder(CustomerOrder order)
    {
      //Do some order processing and ship the product
      _eventBus.Post(new OrderShippedEvent(order.OrderNumber));
    }
  }
```


