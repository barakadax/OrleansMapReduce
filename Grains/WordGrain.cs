using Extensions;
using Extensions.Interfaces;
using Translators.Interfaces;
using GrainInterfaces;

namespace Grains;

public class WordGrain : Grain, IWordGrain
{
    private readonly ITranslatedWordsDictionary _translatedDictionary;
    private readonly IMicrosoftTranslator _translator;
    private string? _translatedWord;

    public WordGrain(IMicrosoftTranslator translator, ITranslatedWordsDictionary translatedDictionary)
    {
        _translatedWord = null;
        _translator = translator;
        _translatedDictionary = translatedDictionary;
    }

    public async Task<ulong> WordCalculate(string? word, string fileName)
    {
        if (_translatedWord!.IsNullOrEmpty() && word!.NotNullNorEmpty() && _translatedDictionary.TranslatedWords.ContainsKey(word!))
        {
            _translatedWord = _translatedDictionary.TranslatedWords[word!];
        }
        else if (_translatedWord!.IsNullOrEmpty() && _translator.CanTranslate())
        {
            _translatedWord = await _translator.GetWordTranslation(word);
            _translatedDictionary.TranslatedWords.TryAdd(word!, _translatedWord!);
        }

        if (_translatedWord!.NotNullNorEmpty())
        {
            word = _translatedWord;
        }

        if (word!.NotNullNorEmpty())
        {
            var numberGrain = GrainFactory.GetGrain<INumberGrain>(fileName + word!.Length);
            numberGrain.Increase();
            return (ulong) word.Length;
        }

        return 0;
    }
}
