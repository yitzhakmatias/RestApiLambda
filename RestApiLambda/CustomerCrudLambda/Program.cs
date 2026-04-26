using Amazon.DynamoDBv2;
using CustomerCrudLambda.Features.Customers;
using CustomerCrudLambda.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.Configure<CustomerStoreOptions>(builder.Configuration.GetSection(CustomerStoreOptions.SectionName));
builder.Services.AddSingleton<ICustomerRepository, DynamoDbCustomerRepository>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { message = "Customer CRUD API running on AWS Lambda" }));
app.MapCustomerEndpoints();

app.Run();

public partial class Program
{
}
