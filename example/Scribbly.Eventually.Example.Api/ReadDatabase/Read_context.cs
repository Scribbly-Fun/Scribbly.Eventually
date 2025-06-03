using System.ComponentModel.DataAnnotations;

namespace Scribbly.Eventually.Example.Api.ReadDatabase;

public class BoxStatus
{
    // Lazy
    [Key]
    public Guid AggregateId { get; set; }
    public int NumberOfBottles { get; set; }
    public ShipmentStatus ShipmentStatus { get; set; }
}

public enum ShipmentStatus
{
    Open,
    Closed,
    Shipped
}