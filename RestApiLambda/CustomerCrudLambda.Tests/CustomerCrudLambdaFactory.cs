using CustomerCrudLambda.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace CustomerCrudLambda.Tests;

internal sealed class CustomerCrudLambdaFactory : WebApplicationFactory<Program>
{
    private readonly Mock<ICustomerRepository> _repositoryMock;

    public CustomerCrudLambdaFactory(Mock<ICustomerRepository> repositoryMock)
    {
        _repositoryMock = repositoryMock;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ICustomerRepository>();
            services.AddSingleton(_repositoryMock.Object);
        });
    }
}
