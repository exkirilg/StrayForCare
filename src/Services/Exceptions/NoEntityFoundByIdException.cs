namespace Services.Exceptions;

public class NoEntityFoundByIdException : Exception
{
    public string PropertyName { get; } = string.Empty;

    public NoEntityFoundByIdException(string message, string propertyName) : base(message)
    {
        PropertyName = propertyName;
    }
}
