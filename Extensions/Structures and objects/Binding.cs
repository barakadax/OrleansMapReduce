namespace Extensions.Interfaces;

public readonly record struct Binding
{
    public required Type Interface { get; init; }
    public required Type Class { get; init; }
}

