using Extensions;
using GrainInterfaces;
using System.Text.RegularExpressions;

namespace Grains;

public partial class FileGrain : Grain, IFileGrain
{
    private readonly Dictionary<ulong, ulong> _result = new ();

    [GeneratedRegex("\\P{L}+")]
    private static partial Regex MyRegex();

    public async Task<Dictionary<ulong, ulong>> ProcessHistogram(string rawText)
    {
        if (_result.NotNullNorEmpty())
        {
            return _result;
        }

        var fileData = MyRegex().Replace(rawText, " ").ToUpper().Split();

        var wordTasks = new List<Task<ulong>>();
        foreach (var task in fileData)
        {
            wordTasks.Add(GrainFactory.GetGrain<IWordGrain>(task).WordCalculate(task));
        }
        await Task.WhenAll(wordTasks);

        var lengthShowing = wordTasks.Select(x => x.Result).Distinct().OrderBy(x => x).ToArray();

        foreach (var length in lengthShowing)
        {
            _ = int.TryParse(length.ToString(), out var grainKey);
            var counter = await GrainFactory.GetGrain<INumberGrain>(grainKey).GetCounter();
            _result.Add(length, counter);
        }

        return _result;
    }
}
