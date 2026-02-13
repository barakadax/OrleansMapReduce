using Extensions;
using Extensions.Interfaces;
using Translators.Interfaces;
using GrainInterfaces;

namespace Grains;

/// <summary>
/// The WordGrain acts as a 'Mapper' in the MapReduce process.
/// Its job is to process a single word, potentially translate it,
/// and then notify a 'Reducer' (NumberGrain) of the resulting word length.
/// </summary>
public class WordGrain : Grain, IWordGrain
{
    private readonly ITranslatedWordsDictionary _translatedDictionary;
    private readonly IMicrosoftTranslator _translator;
    private readonly IGrainFactory _grainFactory;
    
    // State to hold the translated version of this word.
    private string _translatedWord;

    public WordGrain(IMicrosoftTranslator translator, ITranslatedWordsDictionary translatedDictionary, IGrainFactory grainFactory)
    {
        _translatedWord = null;
        _translator = translator;
        _translatedDictionary = translatedDictionary;
        _grainFactory = grainFactory;
    }

    /// <summary>
    /// Processes a single word: translates if necessary, and increments the length counter.
    /// </summary>
    /// <param name="word">The word to process.</param>
    /// <param name="resultIdentifier">A unique ID to isolate this job's counters.</param>
    /// <returns>The length of the processed (potentially translated) word.</returns>
    public async Task<ulong> ProcessWord(string word, string resultIdentifier)
    {
        // 1. Validation.
        if (word.IsNullOrEmpty() || resultIdentifier.IsNullOrEmpty())
        {
            return 0;
        }

        // 2. Translation Logic (Demonstrates integration with external services and shared state).
        if (_translatedWord.IsNullOrEmpty())
        {
            // Check if we already have a translation in our shared dictionary.
            if (_translatedDictionary.TranslatedWords.TryGetValue(word, out var cachedTranslation))
            {
                _translatedWord = cachedTranslation;
            }
            // If not, and we can translate, call the external service.
            else if (_translator.CanTranslate())
            {
                _translatedWord = await _translator.GetWordTranslation(word);
                
                // Cache for future use by other WordGrains.
                _translatedDictionary.TranslatedWords.TryAdd(word, _translatedWord);
            }
        }

        // Use the translated word if available, otherwise fallback to the original.
        var finalWord = _translatedWord.NotNullNorEmpty() ? _translatedWord : word;

        // 3. Increment the Reducer (NumberGrain).
        // Each NumberGrain is responsible for counting words of a specific length.
        var wordLength = (ulong)finalWord.Length;
        var numberGrain = _grainFactory.GetGrain<INumberGrain>($"{resultIdentifier}{wordLength}");
        
        // This is the 'Shuffle' or 'Reduce' trigger in MapReduce terms.
        await numberGrain.Increment();

        return wordLength;
    }
}
