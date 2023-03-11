using Extensions;
using Extensions.Interfaces;
using GrainInterfaces;

namespace Grains;
public class WordGrain : Grain, IWordGrain
{
    private readonly IMicrosoftTranslator _translator;
    private string? _translatedWord;

    public WordGrain(IMicrosoftTranslator translator)
    {
        _translatedWord = null;
        _translator = translator;
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
