﻿using Extensions;
using GrainInterfaces;
using System.Text.RegularExpressions;

namespace Grains;

public partial class FileGrain : Grain, IFileGrain
{
    private readonly Dictionary<ulong, ulong> _result = new ();

    [GeneratedRegex("\\P{L}+")]
    protected static partial Regex MyRegex();

    public Task<Dictionary<ulong, ulong>> GetResultWithoutProcessing()
    {
        return Task.FromResult(_result);
    }

    public async Task<Dictionary<ulong, ulong>> ProcessHistogram(string text, string fileName)
    {
        if (_result.NotNullNorEmpty())
        {
            return _result;
        }

        if (text.IsNullOrEmpty() || fileName.IsNullOrEmpty())
        {
            return null;
        }

        var wordsInFile = MyRegex().Replace(text, " ").ToUpper().Split(' ',StringSplitOptions.RemoveEmptyEntries);

        var wordTasks = new List<Task<ulong>>();
        foreach (var word in wordsInFile)
        {
            wordTasks.Add(GrainFactory.GetGrain<IWordGrain>(word).WordCalculate(word, fileName));
        }
        await Task.WhenAll(wordTasks);

        var lengthShowing = wordTasks.Select(x => x.Result).Distinct().OrderBy(x => x).ToArray();

        foreach (var length in lengthShowing)
        {
            _ = int.TryParse(length.ToString(), out var grainKey);
            var counter = await GrainFactory.GetGrain<INumberGrain>(fileName + grainKey).GetCounter();
            _result.Add(length, counter);
        }

        return _result;
    }
}
