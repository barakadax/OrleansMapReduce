using Extensions;
using Extensions.Interfaces;
using Translators.Interfaces;
using GrainInterfaces;

namespace Grains;

public class WordGrain(IMicrosoftTranslator translator, ITranslatedWordsDictionary translatedDictionary, IGrainFactory grainFactory) : Grain, IWordGrain
{
    private readonly ITranslatedWordsDictionary _translatedDictionary = translatedDictionary;
    private readonly IMicrosoftTranslator _translator = translator;
    private readonly IGrainFactory _grainFactory = grainFactory;

    private string _translatedWord;

    public async Task<ulong> ProcessWord(string word, string resultIdentifier)
    {
        if (word.IsNullOrEmpty() || resultIdentifier.IsNullOrEmpty())
        {
            return 0;
        }

        if (_translatedWord.IsNullOrEmpty())
        {
            if (_translatedDictionary.TranslatedWords.TryGetValue(word, out var cachedTranslation))
            {
                _translatedWord = cachedTranslation;
            }
            else if (_translator.CanTranslate())
            {
                _translatedWord = await _translator.GetWordTranslation(word);
                _translatedDictionary.TranslatedWords.TryAdd(word, _translatedWord);
            }
        }

        var finalWord = _translatedWord.NotNullNorEmpty() ? _translatedWord : word;
        var wordLength = (ulong) finalWord.Length;
        var numberGrain = _grainFactory.GetGrain<INumberGrain>($"{resultIdentifier}{wordLength}");

        await numberGrain.Increment();

        return wordLength;
    }
}
