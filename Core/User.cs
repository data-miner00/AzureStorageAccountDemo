namespace Core;

public sealed class User
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public int Age { get; set; }

    public string[] Emails { get; set; } = [];
}
