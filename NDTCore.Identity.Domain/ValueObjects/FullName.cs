namespace NDTCore.Identity.Domain.ValueObjects;

/// <summary>
/// Full name value object
/// </summary>
public sealed class FullName : IEquatable<FullName>
{
    public string FirstName { get; }
    public string LastName { get; }
    public string DisplayName => $"{FirstName} {LastName}".Trim();

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static FullName Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        return new FullName(firstName.Trim(), lastName.Trim());
    }

    public bool Equals(FullName? other)
    {
        if (other is null) return false;
        return FirstName == other.FirstName && LastName == other.LastName;
    }

    public override bool Equals(object? obj) => obj is FullName other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(FirstName, LastName);
    public override string ToString() => DisplayName;
}

