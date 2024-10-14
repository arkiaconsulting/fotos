namespace Fotos.WebApp.Features.Shared;

internal readonly record struct Name(string Value)
{
    public static Name Create(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value cannot be null or whitespace.", nameof(value))
            : new Name(value);
    }
}