using Extensions;
using GrainInterfaces;
using System.Text.RegularExpressions;

namespace Grains;

/// <summary>
/// The TextGrain acts as the 'Orchestrator' or 'Master' in this MapReduce example.
/// It takes a large block of text, splits it into individual words (Map phase),
/// and then aggregates the results (Reduce phase).
/// </summary>
public partial class TextGrain : Grain, ITextGrain
{
    private readonly IGrainFactory _grainFactory;

    // Holds the final histogram of word lengths.
    // ulong (Length) -> ulong (Count)
    private readonly Dictionary<ulong, ulong> _result = new();

    // Regex to identify non-alphabetical characters for splitting.
    [GeneratedRegex("\\P{L}+")]
    protected static partial Regex WordSplitRegex();

    public TextGrain(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    /// <summary>
    /// Returns the previously calculated results without re-processing.
    /// </summary>
    public Task<Dictionary<ulong, ulong>> GetResults()
    {
        return Task.FromResult(_result);
    }

    /// <summary>
    /// The core MapReduce orchestration logic.
    /// </summary>
    /// <param name="text">The raw text to process.</param>
    /// <param name="resultIdentifier">A unique ID to isolate this job's counters.</param>
    public async Task<Dictionary<ulong, ulong>> ProcessText(string text, string resultIdentifier)
    {
        // Optimization: Return existing result if already calculated.
        if (_result.NotNullNorEmpty())
        {
            return _result;
        }

        // Basic validation.
        if (text.IsNullOrEmpty() || resultIdentifier.IsNullOrEmpty())
        {
            return null;
        }

        // --- MAP PHASE ---
        // 1. Split text into words, normalize to uppercase, and remove empty entries.
        var wordsInText = WordSplitRegex()
            .Replace(text, " ")
            .ToUpper()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // 2. Spawn a WordGrain for each word to process it independently.
        // This demonstrates Orleans' ability to handle massive concurrency.
        var wordTasks = new List<Task<ulong>>();
        foreach (var word in wordsInText)
        {
            // We use the word itself as the Grain ID to potentially reuse grains for the same word.
            wordTasks.Add(_grainFactory.GetGrain<IWordGrain>(word).ProcessWord(word, resultIdentifier));
        }

        // Wait for all Map operations to complete.
        _ = await Task.WhenAll(wordTasks);

        // --- REDUCE PHASE ---
        // 1. Identify all unique word lengths encountered.
        var uniqueLengths = wordTasks
            .Select(x => x.Result)
            .Distinct()
            .OrderBy(x => x);

        // 2. Collect the final counts from the NumberGrains.
        // Each NumberGrain acts as a distributed accumulator for a specific word length.
        foreach (var length in uniqueLengths)
        {
            if (length == 0) continue;

            // The resultIdentifier ensures that counters from different jobs don't collide.
            var counterGrain = _grainFactory.GetGrain<INumberGrain>($"{resultIdentifier}{length}");
            var totalCount = await counterGrain.GetCount();
            
            _result.Add(length, totalCount);
        }

        return _result;
    }
}
