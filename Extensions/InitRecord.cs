namespace Extensions;

public readonly record struct InitRecord
{
    public required string FileName { get; init; }
    public required string FileContent { get; init; }
}
