using Microsoft.EntityFrameworkCore;
using Scribbly.Eventually.Example.Api.EventStream;
using Scribbly.Eventually.Example.Api.ReadDatabase;

namespace Scribbly.Eventually.Example.Api.Projections;

public class ProjectionService<TProjection> : BackgroundService
    where TProjection : class, IProjection
{
    private readonly IServiceProvider _serviceProvider;

    public ProjectionService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {

            var checkpoint = await Get_checkpoint();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var eventContext = scope.ServiceProvider.GetRequiredService<EventContext>();
                    var readContext = scope.ServiceProvider.GetRequiredService<ReadContext>();

                    var transaction = await readContext.Database.BeginTransactionAsync(stoppingToken);

                    var projection = scope.ServiceProvider.GetRequiredService<TProjection>();

                    var events = await GetBatch(checkpoint, projection, eventContext);

                    if (events.Any())
                    {
                        projection.HandleBatch(events.Select(e => new EventMessage(
                            e.AggregateId,
                            e.SequenceNr,
                            e.Event)));

                        checkpoint = events.Last().RowVersion;
                        await WriteCheckpoint(readContext, checkpoint);
                    }
                    else
                    {
                        await Task.Delay(projection.WaitTime, stoppingToken);
                    }
                    await transaction.CommitAsync(stoppingToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
        }
        
    }

    private async Task WriteCheckpoint(ReadContext readContext, ulong checkpoint)
    {
        var checkpointRecord = await readContext.ProjectionCheckpoints
            .FindAsync(typeof(TProjection).Name);
        checkpointRecord!.EventVersion = checkpoint;
        await readContext.SaveChangesAsync();
    }

    private async Task<ulong> Get_checkpoint()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ReadContext>();

        var checkpoint = await context.ProjectionCheckpoints.FindAsync(
            typeof(TProjection).Name);

        if (checkpoint == null)
        {
            checkpoint = new ProjectionCheckpoint
            {
                ProjectionName = typeof(TProjection).Name,
                EventVersion = 0
            };
            context.ProjectionCheckpoints.Add(checkpoint);
            await context.SaveChangesAsync();
        }

        return checkpoint.EventVersion;
    }

    private async Task<IList<EventModel>> GetBatch(
        ulong checkpoint, 
        TProjection projection, 
        EventContext eventContext)
    {
        var typeList = projection.EventTypes.Select(t => t.Name).ToList();

        var batch = await eventContext.Events
            .Where(e => typeList.Contains(e.EventType))
            .Where(e => e.RowVersion > checkpoint)
            .OrderBy(e => e.RowVersion)
            .Take(projection.BatchSize)
            .ToListAsync();

        return batch;
    }
}