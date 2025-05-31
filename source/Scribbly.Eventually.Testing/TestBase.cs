using FluentAssertions;

namespace Scribbly.Eventually;

/// <summary>
/// An abstract class used a base class for routing command handling tests.
/// </summary>
public abstract class TestBase
{
    private readonly GuidId _aggregateId = new (Guid.NewGuid());
    private readonly List<EventMessage> _eventStream = new();
    private readonly List<EventMessage> _newEvents = new();

    /// <summary>
    /// The given phase of the test.  Given 12 books and 10 More books ....
    /// </summary>
    /// <param name="pastEvents"></param>
    protected void Given(params IEvent[] pastEvents)
    {
        foreach (var @event in pastEvents)
        {
            _eventStream.Add(new EventMessage(
                _aggregateId,
                _eventStream.Count + 1,
                @event));
        }
    }

    /// <summary>
    /// The when phase of the test.  When its 9:00pm ....
    /// </summary>
    /// <param name="command"></param>
    protected void When(ICommand command)
    {
        var router = new CommandRouter(_ => _eventStream,
            _newEvents.Add);

        router.Handle(new CommandMessage(_aggregateId, command));
    }

    /// <summary>
    /// The assertion.  I will be up all night reading.
    /// </summary>
    /// <param name="expectedEvents"></param>
    protected void Expect(params object[] expectedEvents)
    {
        _newEvents.Count.Should().Be(expectedEvents.Length);

        for (var i = 0; i < _newEvents.Count; i++)
        {
            var newEvent = _newEvents[i].Event;
            var expectedEvent = expectedEvents[i];

            newEvent.GetType().Should().Be(expectedEvent.GetType());

            try
            {
                newEvent.Should().BeEquivalentTo(expectedEvent);
            }
            catch (InvalidOperationException ex) when (ex.Message.StartsWith("No members were found for comparison."))
            {
                throw;
            }
        }
    }
}