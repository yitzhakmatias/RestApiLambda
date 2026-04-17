using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using CustomerCrudLambda.Models;
using Microsoft.Extensions.Options;

namespace CustomerCrudLambda.Services;

public sealed class DynamoDbCustomerRepository : ICustomerRepository
{
    private readonly IDynamoDBContext _context;
    private readonly DynamoDBOperationConfig _operationConfig;

    public DynamoDbCustomerRepository(IAmazonDynamoDB dynamoDbClient, IOptions<CustomerStoreOptions> options)
    {
        _context = new DynamoDBContext(dynamoDbClient);

        var tableName = Environment.GetEnvironmentVariable("CUSTOMERS_TABLE");
        if (string.IsNullOrWhiteSpace(tableName))
        {
            tableName = options.Value.TableName;
        }

        _operationConfig = new DynamoDBOperationConfig
        {
            OverrideTableName = tableName
        };
    }

    public async Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken)
    {
        var search = _context.ScanAsync<Customer>([], _operationConfig);
        var customers = await search.GetRemainingAsync(cancellationToken);
        return customers;
    }

    public async Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var customer = await _context.LoadAsync<Customer>(id, _operationConfig, cancellationToken);
        return customer;
    }

    public Task CreateAsync(Customer customer, CancellationToken cancellationToken)
    {
        return _context.SaveAsync(customer, _operationConfig, cancellationToken);
    }

    public async Task<Customer?> UpdateAsync(string id, string name, string email, CancellationToken cancellationToken)
    {
        var existing = await _context.LoadAsync<Customer>(id, _operationConfig, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = name;
        existing.Email = email;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveAsync(existing, _operationConfig, cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await _context.LoadAsync<Customer>(id, _operationConfig, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        await _context.DeleteAsync(existing, _operationConfig, cancellationToken);
        return true;
    }
}
