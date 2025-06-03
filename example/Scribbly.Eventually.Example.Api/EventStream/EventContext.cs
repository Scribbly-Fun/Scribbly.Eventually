using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scribbly.Eventually.Example.Api.EventStream;

public class EventContext : DbContext
{
    public EventContext(DbContextOptions<EventContext> options) : base(options)
    { }

    public DbSet<EventModel> Events => Set<EventModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventModelMapping());
    }
}

public class EventModel
{
    public IIdentity AggregateId { get; set; }
    public int SequenceNr { get; set; }
    public string EventType { get; set; }
    public string EventPayload { get; set; }
    public DateTime Timestamp { get; set; }
    public ulong RowVersion { get; set; }
    
    // Possibly: correlation ID, conversation ID

    private IEvent? _event;

    public IEvent Event
    {
        get
        {
            if (_event is null)
            {
                var type = TypeLookup[EventType];
                _event = JsonSerializer.Deserialize(EventPayload, type) as IEvent;
            }
            return _event!;
        }
        set
        {
            _event = value;
            EventType = _event.GetType().Name;
            EventPayload = JsonSerializer.Serialize(_event);
        }
    }

    #region Type dictionary
    
    private static readonly Dictionary<string, Type> TypeLookup = new();

    static EventModel()
    {
        var eventTypes = typeof(IEvent)
            .Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && typeof(IEvent).IsAssignableFrom(type));

        foreach (var eventType in eventTypes)
        {
            TypeLookup[eventType.Name] = eventType;
        }
    }

    #endregion
}

internal class EventModelMapping : IEntityTypeConfiguration<EventModel>
{
    public void Configure(EntityTypeBuilder<EventModel> builder)
    {
        builder.HasKey(e => new { e.AggregateId, e.SequenceNr });

        builder.Ignore(e => e.Event);

        builder.Property(e => e.EventType)
            .HasMaxLength(256)
            .HasColumnType("varchar");

        builder.Property(e => e.EventPayload)
            .HasMaxLength(2048)
            .HasColumnType("varchar");

        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}