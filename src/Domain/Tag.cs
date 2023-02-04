namespace Domain;

public class Tag : IComparable<Tag>
{
    private string _name = string.Empty;

    public ushort TagId { get; private set; }

    public string Name
    {
        get => _name;
        set => _name = value.Trim();
    }

    private Tag()
    {
    }

    public Tag(string name)
    {
        Name = name;
    }

    public int CompareTo(Tag? other)
    {
        return other is null ? -1 : Name.CompareTo(other.Name);
    }
}
