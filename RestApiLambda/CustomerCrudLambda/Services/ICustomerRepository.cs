using CustomerCrudLambda.Models;

namespace CustomerCrudLambda.Services;

public interface ICustomerRepository
{
    Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken);

    Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task CreateAsync(Customer customer, CancellationToken cancellationToken);

    Task<Customer?> UpdateAsync(string id, string name, string email, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}
