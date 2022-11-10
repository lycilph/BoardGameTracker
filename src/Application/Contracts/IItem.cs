namespace BoardGameTracker.Application.Contracts;

public interface IItem
{
    string Id { get; set; }
    string Type { get; set; }
    string PartitionKey { get; }
}
