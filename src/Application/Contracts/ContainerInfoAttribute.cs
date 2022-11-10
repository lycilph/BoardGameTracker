namespace BoardGameTracker.Application.Contracts;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ContainerInfoAttribute : Attribute
{
    public string Name { get; set; } = string.Empty;
    public string PartitionKey { get; set; } = string.Empty;

    public ContainerInfoAttribute(string name, string key)
    {
        Name = name;
        PartitionKey = key;
    }
}
