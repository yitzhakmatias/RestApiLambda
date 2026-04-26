using CustomerCrudLambda.Models;
using CustomerCrudLambda.Services;

namespace CustomerCrudLambda.Features.Customers;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/customers");

        group.MapGet(string.Empty, async (ICustomerRepository repository, CancellationToken cancellationToken) =>
        {
            var customers = await repository.GetAllAsync(cancellationToken);
            return Results.Ok(customers);
        });

        group.MapGet("/{id}", async (string id, ICustomerRepository repository, CancellationToken cancellationToken) =>
        {
            var customer = await repository.GetByIdAsync(id, cancellationToken);
            return customer is null ? Results.NotFound() : Results.Ok(customer);
        });

        group.MapPost(string.Empty, async (CreateCustomerRequest request, ICustomerRepository repository, CancellationToken cancellationToken) =>
        {
            var validatedInput = CustomerRequestValidator.Validate(request.Name, request.Email);
            if (validatedInput is null)
            {
                return Results.BadRequest(new { message = "Name and email are required." });
            }

            var customer = Customer.Create(validatedInput.Value.Name, validatedInput.Value.Email);
            await repository.CreateAsync(customer, cancellationToken);
            return Results.Created($"/customers/{customer.Id}", customer);
        });

        group.MapPut("/{id}", async (string id, UpdateCustomerRequest request, ICustomerRepository repository, CancellationToken cancellationToken) =>
        {
            var validatedInput = CustomerRequestValidator.Validate(request.Name, request.Email);
            if (validatedInput is null)
            {
                return Results.BadRequest(new { message = "Name and email are required." });
            }

            var updated = await repository.UpdateAsync(id, validatedInput.Value.Name, validatedInput.Value.Email, cancellationToken);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        group.MapDelete("/{id}", async (string id, ICustomerRepository repository, CancellationToken cancellationToken) =>
        {
            var deleted = await repository.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
