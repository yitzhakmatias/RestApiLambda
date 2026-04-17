namespace CustomerCrudLambda.Services;

public sealed class CustomerStoreOptions
{
    public const string SectionName = "CustomerStore";

    public string TableName { get; set; } = "Customers";
}
