using System.Diagnostics.CodeAnalysis;

namespace Extensions.Interfaces;

[ExcludeFromCodeCoverage]
public readonly record struct InitRecord
{
    public required string FileName { get; init; }
    public required string FileContent { get; init; }
}
