using Amazon.DynamoDBv2.DataModel;

namespace CustomerCrudLambda.Models;

[DynamoDBTable("Customers")]
public sealed class Customer
{
    [DynamoDBHashKey]
    public string Id { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Name { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Email { get; set; } = string.Empty;

    [DynamoDBProperty]
    public DateTime CreatedAtUtc { get; set; }

    [DynamoDBProperty]
    public DateTime UpdatedAtUtc { get; set; }

    public static Customer Create(string name, string email)
    {
        var now = DateTime.UtcNow;

        return new Customer
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = name,
            Email = email,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
    }
}
