using Extensions;
using GrainInterfaces;
using System.Text.RegularExpressions;

namespace Grains;

public partial class TextGrain(IGrainFactory grainFactory) : Grain, ITextGrain
{
    private readonly IGrainFactory _grainFactory = grainFactory;
    private readonly Dictionary<ulong, ulong> _result = [];

    [GeneratedRegex("\\P{L}+")]
    protected static partial Regex WordSplitRegex();

    public Task<Dictionary<ulong, ulong>> GetResults()
    {
        return Task.FromResult(_result);
    }

    public async Task<Dictionary<ulong, ulong>> ProcessText(string text, string resultIdentifier)
    {
        if (_result.NotNullNorEmpty())
        {
            return _result;
        }

        if (text.IsNullOrEmpty() || resultIdentifier.IsNullOrEmpty())
        {
            return null;
        }

        var wordsInText = WordSplitRegex()
            .Replace(text, " ")
            .ToUpper()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var wordTasks = wordsInText
            .Select(word => _grainFactory.GetGrain<IWordGrain>(word).ProcessWord(word, resultIdentifier))
            .ToList();

        _ = await Task.WhenAll(wordTasks);

        var uniqueLengths = wordTasks
            .Select(x => x.Result)
            .Distinct()
            .OrderBy(x => x);

        var results = await Task.WhenAll(uniqueLengths
            .Where(length => length != 0)
            .Select(async length =>
            {
                var counterGrain = _grainFactory.GetGrain<INumberGrain>($"{resultIdentifier}{length}");
                var count = await counterGrain.GetCount();
                return new { Length = length, Count = count };
            }));

        foreach (var res in results)
        {
            _result.Add(res.Length, res.Count);
        }

        return _result;
    }
}
