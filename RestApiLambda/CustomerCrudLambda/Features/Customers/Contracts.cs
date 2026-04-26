namespace CustomerCrudLambda.Features.Customers;

public sealed record CreateCustomerRequest(string Name, string Email);

public sealed record UpdateCustomerRequest(string Name, string Email);
