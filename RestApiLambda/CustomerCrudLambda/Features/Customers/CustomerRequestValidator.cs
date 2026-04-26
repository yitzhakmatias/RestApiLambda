namespace CustomerCrudLambda.Features.Customers;

public static class CustomerRequestValidator
{
    public static (string Name, string Email)? Validate(string name, string email)
    {
        var normalizedName = name?.Trim();
        var normalizedEmail = email?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName) || string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return null;
        }

        return (normalizedName, normalizedEmail);
    }
}
