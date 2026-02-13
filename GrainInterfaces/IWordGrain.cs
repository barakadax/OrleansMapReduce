namespace GrainInterfaces;
public interface IWordGrain : IGrainWithStringKey
{
    Task<ulong> ProcessWord(string word, string resultIdentifier);
}
