namespace GrainInterfaces;

public interface IFileGrain : IGrainWithStringKey
{
    Task<Dictionary<ulong, ulong>?> ProcessHistogram(string rawText);
}