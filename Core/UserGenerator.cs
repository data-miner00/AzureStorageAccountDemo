namespace Core;

using Bogus;

public sealed class UserGenerator
{
    private readonly Faker<User> faker;

    public UserGenerator()
    {
        this.faker = new Faker<User>()
            .RuleFor(x => x.FirstName, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Username, (s, o) => string.Concat(o.FirstName.ToLowerInvariant(), o.LastName.ToLowerInvariant()))
            .RuleFor(x => x.Age, f => f.Random.Int(18, 99))
            .RuleFor(x => x.Emails, f => f.Make(f.Random.Int(0, 3), () => f.Internet.Email()));
    }
    public Task<User> GenerateAsync()
    {
        return Task.FromResult(this.faker.Generate());
    }

    public Task<IEnumerable<User>> GenerateAsync(int count)
    {
        return Task.FromResult<IEnumerable<User>>(this.faker.Generate(count));
    }
}
