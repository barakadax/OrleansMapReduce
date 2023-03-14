using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Extensions.Interfaces;

public interface ITranslatedWordsDictionary
{
    ConcurrentDictionary<string, string> TranslatedWords { get; init; }
}

[ExcludeFromCodeCoverage]
public readonly record struct TranslatedWordsDictionary : ITranslatedWordsDictionary
{
    public readonly required ConcurrentDictionary<string, string> TranslatedWords { get; init; }
}
