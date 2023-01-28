using Extensions;
using GrainInterfaces;

namespace Grains;
public class WordGrain : Grain, IWordGrain
{
    private readonly MicrosoftTranslator _translator;

    private string? _translatedWord;

    public WordGrain()
    {
        _translator = new();
        _translatedWord = null;
    }

    public async Task<ulong> WordCalculate(string? word)
    {
        if (_translatedWord!.IsNullOrEmpty() && _translator.CanTranslate())
        {
            _translatedWord = await _translator.GetWordTranslation(word);
        }

        if (_translatedWord!.NotNullNorEmpty())
        {
            word = _translatedWord;
        }

        if (word!.NotNullNorEmpty())
        {
            var numberGrain = GrainFactory.GetGrain<INumberGrain>(word!.Length);
            numberGrain.Increase();
            return (ulong) word.Length;
        }

        return 0;
    }
}
