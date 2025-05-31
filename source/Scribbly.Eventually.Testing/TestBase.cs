using FluentAssertions;

namespace Scribbly.Eventually.Testing;

public abstract class TestBase
{
    private readonly GuidId _aggregate_id = new (Guid.NewGuid());
    private readonly List<EventMessage> _event_stream = new();
    private readonly List<EventMessage> _new_events = new();

    protected void Given(params IEvent[] past_events)
    {
        foreach (var @event in past_events)
        {
            _event_stream.Add(new EventMessage(
                _aggregate_id,
                _event_stream.Count + 1,
                @event));
        }
    }

    protected void When(ICommand command)
    {
        var router = new CommandRouter(_ => _event_stream,
            _new_events.Add);

        router.Handle(new CommandMessage(_aggregate_id, command));
    }

    protected void Expect(params object[] expected_events)
    {
        if (_event_stream.Count != expected_events.Length)
        {
            
        }
       
        _new_events.Count.Should().Be(expected_events.Length);

        for (var i = 0; i < _new_events.Count; i++)
        {
            var new_event = _new_events[i].Event;
            var expected_event = expected_events[i];

            new_event.GetType()
                .Should().Be(expected_event.GetType());

            try
            {
                new_event
                    .Should().BeEquivalentTo(expected_event);
            }
            catch (InvalidOperationException ex)
            {
                if (!ex.Message.StartsWith("No members were found for comparison."))
                    throw;
            }
        }
    }
}