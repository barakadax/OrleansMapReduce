namespace GrainInterfaces;

public interface ITextGrain : IGrainWithStringKey
{
    Task<Dictionary<ulong, ulong>> GetResults();
    Task<Dictionary<ulong, ulong>> ProcessText(string text, string resultIdentifier);
}
