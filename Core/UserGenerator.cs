namespace Core;

using Bogus;

/// <summary>
/// A generator for <see cref="User"/>" instance.
/// </summary>
public sealed class UserGenerator
{
    private readonly Faker<User> faker;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserGenerator"/> class.
    /// </summary>
    public UserGenerator()
    {
        this.faker = new Faker<User>()
            .RuleFor(x => x.FirstName, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Username, (s, o) => string.Concat(o.FirstName.ToLowerInvariant(), o.LastName.ToLowerInvariant()))
            .RuleFor(x => x.Age, f => f.Random.Int(18, 99))
            .RuleFor(x => x.Emails, f => [.. f.Make(f.Random.Int(0, 3), () => f.Internet.Email())]);
    }

    /// <summary>
    /// Generates a new <see cref="User"/> instance asynchronously.
    /// </summary>
    /// <returns>The generated user.</returns>
    public Task<User> GenerateAsync()
    {
        return Task.FromResult(this.faker.Generate());
    }

    /// <summary>
    /// Generates a collection of <see cref="User"/> instances asynchronously.
    /// </summary>
    /// <param name="count">The number of instances to be generated.</param>
    /// <returns>The list of users.</returns>
    public Task<IEnumerable<User>> GenerateAsync(int count)
    {
        return Task.FromResult<IEnumerable<User>>(this.faker.Generate(count));
    }
}
