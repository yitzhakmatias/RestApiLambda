using Amazon.DynamoDBv2;
using Amazon.Lambda.AspNetCoreServer.Hosting;
using CustomerCrudLambda.Models;
using CustomerCrudLambda.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.Configure<CustomerStoreOptions>(builder.Configuration.GetSection(CustomerStoreOptions.SectionName));
builder.Services.AddSingleton<ICustomerRepository, DynamoDbCustomerRepository>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { message = "Customer CRUD API running on AWS Lambda" }));

app.MapGet("/customers", async (ICustomerRepository repository, CancellationToken cancellationToken) =>
{
    var customers = await repository.GetAllAsync(cancellationToken);
    return Results.Ok(customers);
});

app.MapGet("/customers/{id}", async (string id, ICustomerRepository repository, CancellationToken cancellationToken) =>
{
    var customer = await repository.GetByIdAsync(id, cancellationToken);
    return customer is null ? Results.NotFound() : Results.Ok(customer);
});

app.MapPost("/customers", async (CreateCustomerRequest request, ICustomerRepository repository, CancellationToken cancellationToken) =>
{
    var customer = Customer.Create(request.Name, request.Email);
    await repository.CreateAsync(customer, cancellationToken);
    return Results.Created($"/customers/{customer.Id}", customer);
});

app.MapPut("/customers/{id}", async (string id, UpdateCustomerRequest request, ICustomerRepository repository, CancellationToken cancellationToken) =>
{
    var updated = await repository.UpdateAsync(id, request.Name, request.Email, cancellationToken);
    return updated is null ? Results.NotFound() : Results.Ok(updated);
});

app.MapDelete("/customers/{id}", async (string id, ICustomerRepository repository, CancellationToken cancellationToken) =>
{
    var deleted = await repository.DeleteAsync(id, cancellationToken);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();
