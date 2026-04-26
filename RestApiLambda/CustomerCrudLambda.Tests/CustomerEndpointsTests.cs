using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CustomerCrudLambda.Features.Customers;
using CustomerCrudLambda.Models;
using CustomerCrudLambda.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace CustomerCrudLambda.Tests;

public class CustomerEndpointsTests
{
    [Fact]
    public async Task GetRoot_ReturnsWelcomeMessage()
    {
        var repositoryMock = CreateRepositoryMock();
        using var factory = new CustomerCrudLambdaFactory(repositoryMock);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("message").GetString().Should().Be("Customer CRUD API running on AWS Lambda");
    }

    [Fact]
    public async Task GetCustomers_ReturnsCustomerList()
    {
        var customers = new List<Customer>
        {
            new() { Id = "c-1", Name = "Ada Lovelace", Email = "ada@example.com" },
            new() { Id = "c-2", Name = "Grace Hopper", Email = "grace@example.com" }
        };

        var repositoryMock = CreateRepositoryMock();
        repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        using var factory = new CustomerCrudLambdaFactory(repositoryMock);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<List<Customer>>();
        body.Should().NotBeNull();
        body!.Count.Should().Be(2);
        body.Should().Contain(x => x.Id == "c-1");
    }

    [Fact]
    public async Task GetCustomerById_WhenFound_ReturnsCustomer()
    {
        var repositoryMock = CreateRepositoryMock();
        repositoryMock
            .Setup(x => x.GetByIdAsync("c-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { Id = "c-1", Name = "Ada Lovelace", Email = "ada@example.com" });

        using var factory = new CustomerCrudLambdaFactory(repositoryMock);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/customers/c-1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Customer>();
        body.Should().NotBeNull();
        body!.Id.Should().Be("c-1");
    }

    [Fact]
    public async Task GetCustomerById_WhenMissing_ReturnsNotFound()
    {
        var repositoryMock = CreateRepositoryMock();
        repositoryMock
            .Setup(x => x.GetByIdAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        using var factory = new CustomerCrudLambdaFactory(repositoryMock);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/customers/missing");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PostCustomer_WithValidPayload_CreatesCustomer()
    {
        var repositoryMock = CreateRepositoryMock();
        repositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var factory = new CustomerCrudLambdaFactory(repositoryMock);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/customers", new CreateCustomerRequest("  Ada Lovelace ", " ada@example.com "));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.OriginalString.Should().StartWith("/customers/");

        repositoryMock.Verify(
            x => x.CreateAsync(
                It.Is<Customer>(c => c.Name == "Ada Lovelace" && c.Email == "ada@example.com"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PostCustomer_WithInvalidPayload_ReturnsBadRequest()
    {
        var repositoryMock = CreateRepositoryMock();
        using var factory = new CustomerCrudLambdaFactory(repositoryMock);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/customers", new CreateCustomerRequest(string.Empty, string.Empty));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        repositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task PutCustomer_WhenFound_UpdatesCustomer()
    {
        var repositoryMock = CreateRepositoryMock();
        repositoryMock
            .Setup(x => x.UpdateAsync("c-1", "Ada Lovelace", "ada@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { Id = "c-1", Name = "Ada Lovelace", Email = "ada@example.com" });

        using var factory = new CustomerCrudLambdaFactory(repositoryMock);
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync("/customers/c-1", new UpdateCustomerRequest(" Ada Lovelace ", " ada@example.com "));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<Customer>();
        body.Should().NotBeNull();
        body!.Id.Should().Be("c-1");
    }

    [Fact]
    public async Task PutCustomer_WhenMissing_ReturnsNotFound()
    {
        var repositoryMock = CreateRepositoryMock();
        repositoryMock
            .Setup(x => x.UpdateAsync("missing", "Ada", "ada@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        using var factory = new CustomerCrudLambdaFactory(repositoryMock);
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync("/customers/missing", new UpdateCustomerRequest("Ada", "ada@example.com"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PutCustomer_WithInvalidPayload_ReturnsBadRequest()
    {
        var repositoryMock = CreateRepositoryMock();
        using var factory = new CustomerCrudLambdaFactory(repositoryMock);
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync("/customers/c-1", new UpdateCustomerRequest(string.Empty, "ada@example.com"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        repositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteCustomer_WhenFound_ReturnsNoContent()
    {
        var repositoryMock = CreateRepositoryMock();
        repositoryMock
            .Setup(x => x.DeleteAsync("c-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        using var factory = new CustomerCrudLambdaFactory(repositoryMock);
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync("/customers/c-1");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCustomer_WhenMissing_ReturnsNotFound()
    {
        var repositoryMock = CreateRepositoryMock();
        repositoryMock
            .Setup(x => x.DeleteAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        using var factory = new CustomerCrudLambdaFactory(repositoryMock);
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync("/customers/missing");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static Mock<ICustomerRepository> CreateRepositoryMock()
    {
        return new Mock<ICustomerRepository>(MockBehavior.Strict);
    }
}
