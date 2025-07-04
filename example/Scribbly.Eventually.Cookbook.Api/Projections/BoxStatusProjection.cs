using Scribbly.Eventually.Cookbook.Api.ReadDatabase;

namespace Scribbly.Eventually.Cookbook.Api.Projections;

public class BoxStatusProjection : IProjection
{
    private readonly ReadContext _dbContext;

    public int BatchSize => 500;
    public TimeSpan WaitTime => TimeSpan.FromMilliseconds(5000);

    public Type[] EventTypes => [
    
        // typeof(BoxCreated),
        // typeof(BeerAdded),
        // typeof(BoxClosed),
        // typeof(BoxShipped)
    ];

    public BoxStatusProjection(ReadContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void HandleBatch(IEnumerable<EventMessage> batch)
    {
        foreach (var eventMessage in batch)
        {
            Handle_message(eventMessage);
        }

        _dbContext.SaveChanges();
    }

    private void Handle_message(EventMessage eventMessage)
    {
        // switch (eventMessage.Event)
        // {
        //     case Box_created:
        //         _dbContext.Box_statuses.Add(new Box_status
        //         {
        //             Aggregate_id = eventMessage.Aggregate_id,
        //             Number_of_bottles = 0,
        //             Shipment_status = Shipment_status.Open
        //         });
        //         break;
        //     case Beer_added:
        //         {
        //             var record = _dbContext.Box_statuses.Find(eventMessage.Aggregate_id);
        //             record!.Number_of_bottles++;
        //             break;
        //         }
        //     case Box_closed:
        //         {
        //             var record = _dbContext.Box_statuses.Find(eventMessage.Aggregate_id);
        //             record!.Shipment_status = Shipment_status.Closed;
        //             break;
        //         }
        //     case Box_shipped:
        //     {
        //         var record = _dbContext.Box_statuses.Find(eventMessage.Aggregate_id);
        //         record!.Shipment_status = Shipment_status.Shipped;
        //         break;
        //     }
        // }
    }
}