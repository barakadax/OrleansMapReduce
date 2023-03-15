namespace Translators.Interfaces;

public interface IMicrosoftTranslator
{
    bool CanTranslate();
    Task<string> GetWordTranslation(string word);
}
