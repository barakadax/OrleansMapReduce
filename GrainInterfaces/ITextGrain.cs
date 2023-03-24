namespace GrainInterfaces;

public interface ITextGrain : IGrainWithStringKey
{
    Task<Dictionary<ulong, ulong>> GetResultWithoutProcessing();
    Task<Dictionary<ulong, ulong>> ProcessHistogram(string text, string name);
}
