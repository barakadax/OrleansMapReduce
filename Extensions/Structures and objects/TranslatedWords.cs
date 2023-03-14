using System.Collections.Concurrent;

namespace Extensions.Interfaces;

public interface ITranslatedWordsDictionary
{
    ConcurrentDictionary<string, string> TranslatedWords { get; init; }
}

public readonly record struct TranslatedWordsDictionary : ITranslatedWordsDictionary
{
    public readonly required ConcurrentDictionary<string, string> TranslatedWords { get; init; }
}
