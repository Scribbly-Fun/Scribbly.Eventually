using Microsoft.AspNetCore.SignalR;

namespace Scribbly.Eventually.Example.Api.EventStream;

public class EventService(
    EventContext eventContext)
{
    private List<EventMessage> _messages = new ();

    public IEnumerable<EventMessage> GetEvents(IIdentity aggregateId)
    {
        return eventContext.Events
            .Where(e => e.AggregateId == (GuidId)aggregateId)
            .OrderBy(e => e.SequenceNr)
            .Select(e => new EventMessage(e.AggregateId, e.SequenceNr, e.Event));
    }

    public void AddEvent(EventMessage @event)
    {
        eventContext.Events.Add(
            new EventModel
            {
                AggregateId = @event.AggregateId,
                SequenceNr = @event.Sequence,
                Event = @event.Event,
                Timestamp = DateTime.UtcNow
            });
        _messages.Add(@event);
    }

    public void Commit()
    {
        eventContext.SaveChanges();

        // foreach (var eventMessage in _messages)
        // {
        //
        //     hub_context.Clients.Groups(eventMessage.Aggregate_id.ToString())
        //         .SendAsync("publish_event", eventMessage.Aggregate_id, eventMessage.Event);
        // }
    }
}